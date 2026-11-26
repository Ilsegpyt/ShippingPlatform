using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.DeactivatePortTerminal;

public sealed class DeactivatePortTerminalCommandHandler(
    IPortTerminalRepository terminalRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<DeactivatePortTerminalCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeactivatePortTerminalCommand command, CancellationToken ct)
    {
        var terminal = await terminalRepository.GetByIdAsync(command.Id, ct);
        if (terminal is null)
            return Result.Failure<bool>("Port terminal not found.");

        var userName = await currentUser.GetUserNameAsync(ct);
        terminal.MarkAsDeleted(currentUser.UserId, userName ?? "User", DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}
