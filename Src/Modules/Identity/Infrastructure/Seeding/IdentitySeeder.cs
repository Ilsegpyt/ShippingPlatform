using Identity.Application.Abstractions;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
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
                }

                _roles.Add(role);
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }
        else
        {
            _logger.LogInformation(
                "Roles already exist — checking Super Admin.");
        }

        if (superAdminRole is null)
        {
            _logger.LogWarning(
                "Super Admin role was not found. Identity seeding stopped.");

            return;
        }

        // Make sure Super Admin always has all current permissions.
        foreach (var permission in PermissionCatalog.All)
        {
            if (!superAdminRole.HasPermission(permission))
                superAdminRole.GrantPermission(permission);
        }

        _roles.Update(superAdminRole);

        await _unitOfWork.SaveChangesAsync(ct);

        // Find existing ApplicationUser.
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

        // Make sure the InternalUser exists.
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