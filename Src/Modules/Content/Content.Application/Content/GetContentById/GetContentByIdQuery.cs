using BuildingBlocks.Application;
using MediatR;

namespace Content.Application.Content.GetContentById;

public sealed record GetContentByIdQuery(
    Guid Id
) : IRequest<Result<ContentDto>>;