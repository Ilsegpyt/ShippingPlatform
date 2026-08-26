using BuildingBlocks.Application;
using BuildingBlocks.Application.Exceptions;
using Identity.Application.Abstractions;
using Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Authentication;

public sealed class IdentityUserService : IIdentityUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityUserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // Create a new Identity user.
    public async Task<Guid> CreateUserAsync(string email, string defaultPassword, bool isInternal, string? phone, CancellationToken ct = default)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            Kind = isInternal
                ? UserKind.Internal
                : UserKind.Customer,
            IsActive = true,
            PhoneNumber = phone
        };

        var result = await _userManager.CreateAsync(
            user,
            defaultPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(e => e.Description));

            if (result.Errors.Any(e =>
                e.Code is "DuplicateEmail" or "DuplicateUserName"))
            {
                throw new ConflictException(errors);
            }

            throw new InvalidOperationException(
                $"Failed to create identity user: {errors}");
        }

        return user.Id;
    }

    // Validate user credentials.
    public async Task<Guid?> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null || !user.IsActive)
            return null;

        var valid = await _userManager.CheckPasswordAsync(user, password);

        return valid
            ? user.Id
            : null;
    }

    // Activate or deactivate a user.
    public async Task SetActiveAsync(Guid userId, bool isActive, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(
            userId.ToString());

        if (user is null)
            return;

        if (user.IsActive == isActive)
            return;

        user.IsActive = isActive;

        await _userManager.UpdateAsync(user);
    }

    // Check whether a user is active.
    public async Task<bool> IsActiveAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(
            userId.ToString());

        return user is not null && user.IsActive;
    }

    public async Task<Result> UpdateEmailAsync(Guid userId, string email,
    CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Result.Failure("Identity user not found.");

        user.Email = email;
        user.UserName = email;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(e => e.Description));

            return Result.Failure(errors);
        }

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(
    Guid userId,
    string newPassword,
    CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Result.Failure("Identity user not found.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(
            user,
            token,
            newPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(e => e.Description));

            return Result.Failure(errors);
        }

        return Result.Success();
    }
    public async Task<Result> DeleteUserAsync(
    Guid userId,
    CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(
            userId.ToString());

        if (user is null)
            return Result.Failure("Identity user not found.");

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(e => e.Description));

            return Result.Failure(errors);
        }

        return Result.Success();
    }

}