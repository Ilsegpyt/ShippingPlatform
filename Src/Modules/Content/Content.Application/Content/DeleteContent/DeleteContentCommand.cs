using BuildingBlocks.Application;
using MediatR;

namespace Content.Application.Content.DeleteContent;

public sealed record DeleteContentCommand(
    Guid Id) : IRequest<Result>;