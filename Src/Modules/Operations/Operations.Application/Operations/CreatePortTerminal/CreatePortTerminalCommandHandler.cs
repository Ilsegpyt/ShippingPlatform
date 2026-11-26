using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Entities.Fcl;

namespace Operations.Application.Operations.CreatePortTerminal;

public sealed class CreatePortTerminalCommandHandler(
    IPortTerminalRepository terminalRepository,
    IOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<CreatePortTerminalCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePortTerminalCommand command, CancellationToken ct)
    {
        if (!await operationRepository.ActivePortExistsAsync(command.PortId, ct))
            return Result.Failure<Guid>("Port does not exist or is inactive.");

        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Guid>("Terminal name is required.");

        if (await terminalRepository.NameExistsAsync(command.PortId, name, null, ct))
            return Result.Failure<Guid>($"Terminal '{name}' already exists for this port.");

        if (!string.IsNullOrWhiteSpace(command.Code) &&
            await terminalRepository.CodeExistsAsync(command.PortId, command.Code, null, ct))
        {
            return Result.Failure<Guid>($"Terminal code '{command.Code.Trim()}' already exists for this port.");
        }

        var terminal = PortTerminal.Create(command.PortId, name, command.Code, command.ArabicName);
        await terminalRepository.AddAsync(terminal, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(terminal.Id);
    }
}
