using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.UpdateFtlTerminal;

public sealed class UpdateFtlTerminalCommandHandler(
    IFtlTerminalRepository terminalRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateFtlTerminalCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateFtlTerminalCommand command, CancellationToken ct)
    {
        var terminal = await terminalRepository.GetByIdAsync(command.Id, ct);
        if (terminal is null) return Result.Failure<bool>("FTL Terminal not found.");

        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<bool>("Terminal name is required.");

        if (await terminalRepository.NameExistsAsync(name, command.Id, ct))
            return Result.Failure<bool>($"FTL Terminal '{name}' already exists.");

        terminal.Update(name, command.IsActive);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}
