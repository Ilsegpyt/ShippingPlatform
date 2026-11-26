using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.UpdatePortTerminal;

public sealed class UpdatePortTerminalCommandHandler(
    IPortTerminalRepository terminalRepository,
    IOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<UpdatePortTerminalCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdatePortTerminalCommand command, CancellationToken ct)
    {
        var terminal = await terminalRepository.GetByIdAsync(command.Id, ct);
        if (terminal is null)
            return Result.Failure<bool>("Port terminal not found.");

        if (!await operationRepository.ActivePortExistsAsync(command.PortId, ct))
            return Result.Failure<bool>("Port does not exist or is inactive.");

        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<bool>("Terminal name is required.");

        if (await terminalRepository.NameExistsAsync(command.PortId, name, command.Id, ct))
            return Result.Failure<bool>($"Terminal '{name}' already exists for this port.");

        if (!string.IsNullOrWhiteSpace(command.Code) &&
            await terminalRepository.CodeExistsAsync(command.PortId, command.Code, command.Id, ct))
        {
            return Result.Failure<bool>($"Terminal code '{command.Code.Trim()}' already exists for this port.");
        }

        terminal.Update(command.PortId, name, command.Code, command.ArabicName, command.IsActive);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}
