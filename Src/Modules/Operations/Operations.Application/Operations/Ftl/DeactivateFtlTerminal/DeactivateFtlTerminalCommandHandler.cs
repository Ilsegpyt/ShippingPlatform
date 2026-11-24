using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.DeactivateFtlTerminal;

public sealed class DeactivateFtlTerminalCommandHandler(
    IFtlTerminalRepository terminalRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<DeactivateFtlTerminalCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeactivateFtlTerminalCommand command, CancellationToken ct)
    {
        var terminal = await terminalRepository.GetByIdAsync(command.Id, ct);
        if (terminal is null) return Result.Failure<bool>("FTL Terminal not found.");

        var userName = await currentUser.GetUserNameAsync(ct);
        terminal.MarkAsDeleted(currentUser.UserId, userName ?? "User", DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}
