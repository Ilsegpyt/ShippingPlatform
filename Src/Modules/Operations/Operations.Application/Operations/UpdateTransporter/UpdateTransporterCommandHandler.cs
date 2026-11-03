using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.UpdateTransporter;

public sealed class UpdateTransporterCommandHandler(
    ITransporterRepository transporterRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateTransporterCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateTransporterCommand command, CancellationToken ct)
    {
        var transporter = await transporterRepository.GetByIdAsync(command.Id, ct);
        if (transporter is null) return Result.Failure<bool>("Transporter not found.");

        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name)) return Result.Failure<bool>("Transporter name is required.");

        if (await transporterRepository.TransporterNameExistsAsync(name, command.Id, ct))
            return Result.Failure<bool>($"Transporter '{name}' already exists.");

        transporter.Update(name, command.IsActive);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
