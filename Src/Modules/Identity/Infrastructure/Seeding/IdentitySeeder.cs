using Identity.Application.Abstractions;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Seeding;

public sealed class IdentitySeeder
{
    private static readonly (string Name, string Description)[] BaselineRoles =
    {
        ("Super Admin", "Full system access with all permissions"),
        ("Account Manager", "Manage assigned customers, shipments, and related operations"),
        ("Operations", "Manage scheduling, shipments, locations, and ports"),
        ("Executive Maritime Transportation", "Maritime transport data entry and shipment processing"),
        ("Supervisor Maritime Transportation", "Manage maritime transport operations, dashboards, reopen shipments and reports"),
        ("FTL Operation Executive", "FTL operations data entry and trip processing"),
        ("Sales", "Manage leads, quotations, and customer inquiries"),
        ("Viewer", "Read-only access to view data"),
        ("Finance Manager", "Oversees Financial functions")
    };

    private readonly IRoleRepository _roles;
    private readonly IInternalUserRepository _internalUsers;
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SeedOptions _seedOptions;
    private readonly ILogger<IdentitySeeder> _logger;

    public IdentitySeeder(
        IRoleRepository roles,
        IInternalUserRepository internalUsers,
        IIdentityUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        IOptions<SeedOptions> seedOptions,
        ILogger<IdentitySeeder> logger)
    {
        _roles = roles;
        _internalUsers = internalUsers;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _seedOptions = seedOptions.Value;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        var existingRoles = await _roles.GetAllAsync(ct);

        Role? superAdminRole = existingRoles
            .FirstOrDefault(x => x.Name == "Super Admin");

        Role? accountManagerRole = existingRoles
            .FirstOrDefault(x => x.Name == "Account Manager");

        // Create baseline roles if they do not exist.
        if (existingRoles.Count == 0)
        {
            _logger.LogInformation(
                "Seeding baseline roles...");

            foreach (var (name, description) in BaselineRoles)
            {
                var role = Role.Create(name, description);

                if (name == "Super Admin")
                {
                    foreach (var permission in PermissionCatalog.All)
                        role.GrantPermission(permission);

                    superAdminRole = role;
                }
                else if (name == "Account Manager")
                {
                    foreach (var permission in PermissionCatalog.AccountManagerPermissions)
                        role.GrantPermission(permission);

                    accountManagerRole = role;
                }
                else if (name == "FTL Operation Executive")
                {
                    foreach (var permission in PermissionCatalog.FtlOperationExecutivePermissions)
                        role.GrantPermission(permission);
                }
                else if (name == "Executive Maritime Transportation")
                {
                    foreach (var permission in PermissionCatalog.ExecutiveMaritimeTransportationPermissions)
                        role.GrantPermission(permission);
                }

                _roles.Add(role);
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }
        else
        {
            _logger.LogInformation(
                "Roles already exist — checking baseline role permissions.");
        }

        // Re-load roles after possible creation.
        existingRoles = await _roles.GetAllAsync(ct);

        superAdminRole = existingRoles
            .FirstOrDefault(x => x.Name == "Super Admin");

        accountManagerRole = existingRoles
            .FirstOrDefault(x => x.Name == "Account Manager");

        if (superAdminRole is null)
        {
            _logger.LogWarning(
                "Super Admin role was not found. Identity seeding stopped.");

            return;
        }

        // ---------------------------------------------------------
        // Super Admin
        // ---------------------------------------------------------
        // Make sure Super Admin always has all current permissions.
        foreach (var permission in PermissionCatalog.All)
        {
            if (!superAdminRole.HasPermission(permission))
                superAdminRole.GrantPermission(permission);
        }

        _roles.Update(superAdminRole);

        // ---------------------------------------------------------
        // Account Manager
        // ---------------------------------------------------------
        // Make sure Account Manager always has its baseline permissions.
        if (accountManagerRole is null)
        {
            _logger.LogWarning(
                "Account Manager role was not found.");
        }
        else
        {
            foreach (var permission in PermissionCatalog.AccountManagerPermissions)
            {
                if (!accountManagerRole.HasPermission(permission))
                    accountManagerRole.GrantPermission(permission);
            }

            _roles.Update(accountManagerRole);
        }

        // ---------------------------------------------------------
        // Migrate legacy role names to standardized names
        // ---------------------------------------------------------
        var legacyOceanManager = existingRoles
            .FirstOrDefault(x => x.Name == "Ocean Operations Manager");

        if (legacyOceanManager is not null)
        {
            _logger.LogInformation(
                "Migrating role 'Ocean Operations Manager' to 'Supervisor Maritime Transportation'.");

            legacyOceanManager.Rename(
                "Supervisor Maritime Transportation",
                "Manage maritime transport operations, dashboards, reopen shipments and reports");

            _roles.Update(legacyOceanManager);
        }

        var legacyOceanSpecialist = existingRoles
            .FirstOrDefault(x => x.Name == "Ocean Operations Specialist");

        if (legacyOceanSpecialist is not null)
        {
            _logger.LogInformation(
                "Migrating role 'Ocean Operations Specialist' to 'Executive Maritime Transportation'.");

            legacyOceanSpecialist.Rename(
                "Executive Maritime Transportation",
                "Maritime transport data entry and shipment processing");

            _roles.Update(legacyOceanSpecialist);
        }

        // ---------------------------------------------------------
        // FTL Operation Executive
        // ---------------------------------------------------------
        var ftlExecutiveRole = existingRoles
            .FirstOrDefault(x => x.Name == "FTL Operation Executive");

        if (ftlExecutiveRole is null)
        {
            _logger.LogInformation(
                "FTL Operation Executive role was not found — creating it.");

            ftlExecutiveRole = Role.Create(
                "FTL Operation Executive",
                "FTL operations data entry and trip processing");

            foreach (var permission in PermissionCatalog.FtlOperationExecutivePermissions)
                ftlExecutiveRole.GrantPermission(permission);

            _roles.Add(ftlExecutiveRole);
        }
        else
        {
            foreach (var permission in PermissionCatalog.FtlOperationExecutivePermissions)
            {
                if (!ftlExecutiveRole.HasPermission(permission))
                    ftlExecutiveRole.GrantPermission(permission);
            }

            _roles.Update(ftlExecutiveRole);
        }

        // ---------------------------------------------------------
        // Executive Maritime Transportation
        // ---------------------------------------------------------
        var executiveMaritime = existingRoles
            .FirstOrDefault(x => x.Name == "Executive Maritime Transportation");

        if (executiveMaritime is not null)
        {
            foreach (var permission in PermissionCatalog.ExecutiveMaritimeTransportationPermissions)
            {
                if (!executiveMaritime.HasPermission(permission))
                    executiveMaritime.GrantPermission(permission);
            }

            _roles.Update(executiveMaritime);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        // ---------------------------------------------------------
        // Super Admin ApplicationUser
        // ---------------------------------------------------------
        var applicationUser = await _userManager.FindByEmailAsync(
            _seedOptions.SuperAdminEmail);

        Guid userId;

        if (applicationUser is null)
        {
            _logger.LogInformation(
                "Super Admin ApplicationUser not found — creating it.");

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = _seedOptions.SuperAdminEmail,
                Email = _seedOptions.SuperAdminEmail,
                EmailConfirmed = true,
                Kind = UserKind.Internal,
                IsActive = true
            };

            var createResult = await _userManager.CreateAsync(
                user,
                _seedOptions.SuperAdminPassword);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    createResult.Errors.Select(x => x.Description));

                throw new InvalidOperationException(
                    $"Failed to create Super Admin ApplicationUser: {errors}");
            }

            userId = user.Id;
        }
        else
        {
            userId = applicationUser.Id;

            _logger.LogInformation(
                "Super Admin ApplicationUser already exists.");
        }

        // ---------------------------------------------------------
        // Super Admin InternalUser
        // ---------------------------------------------------------
        var internalUser = await _internalUsers.GetByUserIdAsync(
            userId,
            ct);

        if (internalUser is null)
        {
            internalUser = InternalUser.Create(
                userId,
                superAdminRole.Id,
                "Super Admin",
                _seedOptions.SuperAdminEmail,
                null);

            _internalUsers.Add(internalUser);

            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Super Admin InternalUser created.");
        }
        else
        {
            _logger.LogInformation(
                "Super Admin InternalUser already exists.");
        }

        _logger.LogInformation(
            "Identity seeding completed. Super Admin login: {Email}",
            _seedOptions.SuperAdminEmail);
    }
}
