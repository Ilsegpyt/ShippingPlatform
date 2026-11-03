using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Application.Operations.CreatePort;

public sealed class CreatePortCommandHandler(
    IPortRepository portRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<CreatePortCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePortCommand command, CancellationToken ct)
    {
        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Guid>("Port name is required.");

        if (await portRepository.PortNameExistsAsync(name, null, ct))
            return Result.Failure<Guid>($"Port '{name}' already exists.");

        if (!string.IsNullOrWhiteSpace(command.Code) && await portRepository.PortCodeExistsAsync(command.Code, null, ct))
            return Result.Failure<Guid>($"Port code '{command.Code.Trim()}' already exists.");

        var port = Port.Create(name, command.Code);
        await portRepository.AddAsync(port, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(port.Id);
    }
}
