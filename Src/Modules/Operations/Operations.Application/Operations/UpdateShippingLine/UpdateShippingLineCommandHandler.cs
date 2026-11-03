using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.UpdateShippingLine;

public sealed class UpdateShippingLineCommandHandler(
    IShippingLineRepository shippingLineRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateShippingLineCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateShippingLineCommand command, CancellationToken ct)
    {
        var line = await shippingLineRepository.GetByIdAsync(command.Id, ct);
        if (line is null) return Result.Failure<bool>("Shipping line not found.");

        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name)) return Result.Failure<bool>("Shipping line name is required.");

        if (await shippingLineRepository.ShippingLineNameExistsAsync(name, command.Id, ct))
            return Result.Failure<bool>($"Shipping line '{name}' already exists.");

        line.Update(name, command.Abbreviation, command.IsActive);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
