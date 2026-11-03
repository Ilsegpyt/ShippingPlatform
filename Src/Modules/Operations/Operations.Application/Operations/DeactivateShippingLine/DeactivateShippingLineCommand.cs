using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.DeactivateShippingLine;

public sealed record DeactivateShippingLineCommand(Guid Id) : IRequest<Result<bool>>;
