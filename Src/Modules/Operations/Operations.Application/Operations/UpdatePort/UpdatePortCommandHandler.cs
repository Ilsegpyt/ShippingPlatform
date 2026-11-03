using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.UpdatePort;

public sealed class UpdatePortCommandHandler(
    IPortRepository portRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<UpdatePortCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdatePortCommand command, CancellationToken ct)
    {
        var port = await portRepository.GetByIdAsync(command.Id, ct);
        if (port is null) return Result.Failure<bool>("Port not found.");

        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name)) return Result.Failure<bool>("Port name is required.");

        if (await portRepository.PortNameExistsAsync(name, command.Id, ct))
            return Result.Failure<bool>($"Port '{name}' already exists.");

        if (!string.IsNullOrWhiteSpace(command.Code) && await portRepository.PortCodeExistsAsync(command.Code, command.Id, ct))
            return Result.Failure<bool>($"Port code '{command.Code.Trim()}' already exists.");

        port.Update(name, command.Code, command.IsActive);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
