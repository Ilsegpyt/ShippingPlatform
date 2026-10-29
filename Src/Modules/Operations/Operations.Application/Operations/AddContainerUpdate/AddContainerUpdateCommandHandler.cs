using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Operations.GetAllOperations;
using Operations.Domain.Entities;

namespace Operations.Application.Operations.AddContainerUpdate;

public sealed class AddContainerUpdateCommandHandler(
    IOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<AddContainerUpdateCommand, Result<ContainerUpdateDto>>
{
    public async Task<Result<ContainerUpdateDto>> Handle(
        AddContainerUpdateCommand command,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Details))
            return Result.Failure<ContainerUpdateDto>("Update details cannot be empty.");

        var container = await operationRepository.GetContainerByIdAsync(command.ContainerId, ct);
        if (container is null)
            return Result.Failure<ContainerUpdateDto>("Container not found.");

        var userName = await currentUser.GetUserNameAsync(ct);
        var userId = currentUser.UserId.ToString();
        var utcNow = DateTime.UtcNow;

        var update = ContainerUpdate.Create(
            container.Id,
            command.Details.Trim(),
            userId,
            userName,
            utcNow);

        await operationRepository.AddContainerUpdateAsync(update, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new ContainerUpdateDto(
            update.Id,
            update.OperationContainerId,
            update.Details,
            update.CreatedAtUtc,
            update.CreatedByUserId,
            update.CreatedByName));
    }
}
