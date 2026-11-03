using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Application.Operations.CreateShippingLine;

public sealed class CreateShippingLineCommandHandler(
    IShippingLineRepository shippingLineRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<CreateShippingLineCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateShippingLineCommand command, CancellationToken ct)
    {
        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Guid>("Shipping line name is required.");

        if (await shippingLineRepository.ShippingLineNameExistsAsync(name, null, ct))
            return Result.Failure<Guid>($"Shipping line '{name}' already exists.");

        var line = ShippingLine.Create(name, command.Abbreviation);
        await shippingLineRepository.AddAsync(line, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(line.Id);
    }
}
