using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.UpdateShippingLine;

public sealed record UpdateShippingLineCommand(Guid Id, string Name, string? Abbreviation, bool IsActive) : IRequest<Result<bool>>;
