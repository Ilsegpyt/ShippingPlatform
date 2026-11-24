using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Entities.Ftl;

namespace Operations.Application.Operations.Ftl.CreateFtlTerminal;

public sealed class CreateFtlTerminalCommandHandler(
    IFtlTerminalRepository terminalRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<CreateFtlTerminalCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateFtlTerminalCommand command, CancellationToken ct)
    {
        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Guid>("Terminal name is required.");

        if (await terminalRepository.NameExistsAsync(name, null, ct))
            return Result.Failure<Guid>($"FTL Terminal '{name}' already exists.");

        var terminal = FtlTerminal.Create(name);
        await terminalRepository.AddAsync(terminal, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(terminal.Id);
    }
}
