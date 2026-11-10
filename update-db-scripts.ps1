<#
.SYNOPSIS
    Updates database/init/*.sql scripts when new EF Core migrations are added.
.DESCRIPTION
    Checks each module's C# migrations against database/init/<Module>.sql.
    If new migrations are detected, it generates their idempotent SQL using `dotnet ef`
    and appends them automatically to the corresponding SQL file.
.PARAMETER Module
    Optional: The specific module name to update (e.g. Operations, Customers, Identity).
    If omitted, all modules are checked and updated.
.EXAMPLE
    .\update-db-scripts.ps1
    .\update-db-scripts.ps1 -Module Operations
#>
param(
    [string]$Module = "All"
)

$ErrorActionPreference = "Stop"

$repoRoot = $PSScriptRoot
if (-not $repoRoot) { $repoRoot = Get-Location }

$apiProject = Join-Path $repoRoot "Src\Api\Api.csproj"
$initFolder = Join-Path $repoRoot "database\init"

$modules = @(
    @{ Name = "Content"; Project = "Src\Modules\Content\Content.Infrastructure\Content.Infrastructure.csproj"; Context = "ContentDbContext"; Sql = "01_Content.sql" },
    @{ Name = "Customers"; Project = "Src\Modules\Customers\Infrastructure\Customers.Infrastructure.csproj"; Context = "CustomersDbContext"; Sql = "02_Customers.sql" },
    @{ Name = "Identity"; Project = "Src\Modules\Identity\Infrastructure\Identity.Infrastructure.csproj"; Context = "IdentityDbContext"; Sql = "03_Identity.sql" },
    @{ Name = "Notifications"; Project = "Src\Modules\Notifications\Notifications.Infrastructure\Notifications.Infrastructure.csproj"; Context = "NotificationsDbContext"; Sql = "04_Notifications.sql" },
    @{ Name = "Operations"; Project = "Src\Modules\Operations\Operations.Infrastructure\Operations.Infrastructure.csproj"; Context = "OperationsDbContext"; Sql = "05_Operations.sql" },
    @{ Name = "Reports"; Project = "Src\Modules\Reports\Reports.Infrastructure\Reports.Infrastructure.csproj"; Context = "ReportsDbContext"; Sql = "06_Reports.sql" },
    @{ Name = "Website"; Project = "Src\Modules\Website\Website.Infrastructure\Website.Infrastructure.csproj"; Context = "WebsiteDbContext"; Sql = "07_Website.sql" }
)

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host " ShippingPlatform - DB Scripts Sync Tool" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

foreach ($mod in $modules) {
    if ($Module -ne "All" -and $mod.Name -ne $Module) {
        continue
    }

    $modName = $mod.Name
    $sqlFile = Join-Path $initFolder $mod.Sql
    $projPath = Join-Path $repoRoot $mod.Project
    $projDir = Split-Path $projPath -Parent
    
    $migrationsDir = Join-Path $projDir "Migrations"
    if (-not (Test-Path $migrationsDir)) {
        $migrationsDir = Join-Path $projDir "Persistence\Migrations"
    }

    if (-not (Test-Path $migrationsDir)) {
        Write-Host "[-] $($modName): Migrations folder not found." -ForegroundColor Yellow
        continue
    }

    # Get all C# migration names ordered by timestamp
    $csMigrations = Get-ChildItem -Path $migrationsDir -Filter "*.cs" |
        Where-Object { $_.Name -notmatch "Designer|Snapshot" } |
        Sort-Object Name |
        ForEach-Object { $_.BaseName }

    if (-not (Test-Path $sqlFile)) {
        Write-Host "[!] $($modName): SQL file not found at $sqlFile" -ForegroundColor Yellow
        continue
    }

    $sqlContent = [System.IO.File]::ReadAllText($sqlFile, [System.Text.Encoding]::UTF8)

    # Find missing migrations
    $missingMigrations = @()
    $lastFoundInSql = $null
    foreach ($m in $csMigrations) {
        if ($sqlContent -match [regex]::Escape($m)) {
            $lastFoundInSql = $m
        } else {
            $missingMigrations += $m
        }
    }

    if ($missingMigrations.Count -eq 0) {
        Write-Host "[OK] $($modName) is already up to date ($($csMigrations.Count) migrations)." -ForegroundColor Green
        continue
    }

    Write-Host "[*] $($modName) has $($missingMigrations.Count) new migration(s):" -ForegroundColor Magenta
    $missingMigrations | ForEach-Object { Write-Host "    -> $_" -ForegroundColor Gray }

    # Generate SQL script
    $tempScript = [System.IO.Path]::GetTempFileName() + ".sql"
    try {
        Write-Host "    Generating idempotent SQL script via dotnet ef..." -ForegroundColor Cyan

        $efArgs = @(
            "ef", "migrations", "script"
        )
        if ($lastFoundInSql) {
            $efArgs += $lastFoundInSql
        }
        $efArgs += @(
            "--idempotent",
            "--project", $projPath,
            "--startup-project", $apiProject,
            "--context", $mod.Context,
            "--output", $tempScript
        )

        $proc = Start-Process -FilePath "dotnet" -ArgumentList $efArgs -NoNewWindow -Wait -PassThru
        if ($proc.ExitCode -ne 0) {
            Write-Host "[-] Failed to generate script for $($modName) (Exit Code $($proc.ExitCode))." -ForegroundColor Red
            continue
        }

        if ((Test-Path $tempScript) -and (Get-Item $tempScript).Length -gt 0) {
            $newSql = [System.IO.File]::ReadAllText($tempScript, [System.Text.Encoding]::UTF8)
            
            # Append cleanly to SQL file
            $separator = "`r`n`r`n/* ===========================================================================`r`n   Added by update-db-scripts.ps1: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')`r`n   =========================================================================== */`r`n`r`n"
            [System.IO.File]::AppendAllText($sqlFile, $separator + $newSql, [System.Text.Encoding]::UTF8)

            Write-Host "[+] Successfully appended $($missingMigrations.Count) migration(s) to database/init/$($mod.Sql)" -ForegroundColor Green
        } else {
            Write-Host "[-] Generated script was empty for $($modName)." -ForegroundColor Yellow
        }
    }
    finally {
        if (Test-Path $tempScript) {
            Remove-Item $tempScript -Force -ErrorAction SilentlyContinue
        }
    }
}

Write-Host "`nAll checks completed.`n" -ForegroundColor Cyan
