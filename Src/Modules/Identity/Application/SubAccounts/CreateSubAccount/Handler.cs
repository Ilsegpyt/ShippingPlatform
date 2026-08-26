using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Domain;
using Identity.Domain.Exceptions;
using Identity.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace Identity.Application.SubAccounts.CreateSubAccount;

public sealed class CreateSubAccountHandler
    : IRequestHandler<CreateSubAccountCommand, Result<CreateSubAccountResponse>>
{
    private readonly IIdentityUserService _identityUsers;
    private readonly ISubAccountRepository _subAccounts;
    private readonly IIdentityUnitOfWork _identityUnitOfWork;
    private readonly SubAccountOptions _options;

    public CreateSubAccountHandler(
        IIdentityUserService identityUsers,
        ISubAccountRepository subAccounts,
        IIdentityUnitOfWork identityUnitOfWork,
        IOptions<SubAccountOptions> options)
    {
        _identityUsers = identityUsers;
        _subAccounts = subAccounts;
        _identityUnitOfWork = identityUnitOfWork;
        _options = options.Value;
    }

    public async Task<Result<CreateSubAccountResponse>> Handle(
        CreateSubAccountCommand command,
        CancellationToken ct)
    {
        if (command.GrantFullScope && command.Scopes.Count > 0)
        {
            return Result.Failure<CreateSubAccountResponse>(
                "Scopes cannot be provided when GrantFullScope is true.");
        }

        var scopes = new List<PermissionScope>();

        if (!command.GrantFullScope)
        {
            foreach (var input in command.Scopes)
            {
                try
                {
                    scopes.Add(
                        PermissionScope.Create(
                            input.Category,
                            input.Service,
                            input.Type));
                }
                catch (IdentityDomainException ex)
                {
                    return Result.Failure<CreateSubAccountResponse>(
                        $"Invalid scope ({input.Category}/{input.Service}/{input.Type}): {ex.Message}");
                }
            }
        }

        await _identityUnitOfWork.BeginTransactionAsync(ct);

        try
        {
            var userId = await _identityUsers.CreateUserAsync(
                command.Email,
                _options.DefaultPassword,
                isInternal: false,
                null,
                ct);

            var scopeType = command.GrantFullScope
                ? ScopeType.Full
                : ScopeType.Custom;

            var subAccount = SubAccount.Create(
                command.OrganizationId,
                userId,
                command.Name,
                command.Email,
                scopeType,
                SubAccountStatus.Active);

            if (!command.GrantFullScope)
            {
                foreach (var scope in scopes)
                {
                    subAccount.AddScope(scope);
                }
            }

            _subAccounts.Add(subAccount);

            await _identityUnitOfWork.SaveChangesAsync(ct);

            await _identityUnitOfWork.CommitTransactionAsync(ct);

            return Result.Success(
                new CreateSubAccountResponse(
                    subAccount.Id,
                    _options.DefaultPassword));
        }
        catch
        {
            await _identityUnitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }
}