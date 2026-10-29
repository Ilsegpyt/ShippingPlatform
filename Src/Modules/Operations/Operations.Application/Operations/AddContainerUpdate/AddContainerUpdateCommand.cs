using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Operations.GetAllOperations;

namespace Operations.Application.Operations.AddContainerUpdate;

public sealed record AddContainerUpdateCommand(
    Guid ContainerId,
    string Details
) : IRequest<Result<ContainerUpdateDto>>;
