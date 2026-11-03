using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.CreateShippingLine;

public sealed record CreateShippingLineCommand(string Name, string? Abbreviation = null) : IRequest<Result<Guid>>;
