using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.UpdateShippingLine;

public sealed record UpdateShippingLineCommand(Guid Id, string Name, bool IsActive) : IRequest<Result<bool>>;
