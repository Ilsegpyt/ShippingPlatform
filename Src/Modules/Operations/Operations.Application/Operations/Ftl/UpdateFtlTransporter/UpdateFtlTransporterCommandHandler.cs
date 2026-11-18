using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.UpdateFtlTransporter;

public sealed class UpdateFtlTransporterCommandHandler(
    IFtlTransporterRepository transporterRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateFtlTransporterCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateFtlTransporterCommand command, CancellationToken ct)
    {
        var transporter = await transporterRepository.GetByIdAsync(command.Id, ct);
        if (transporter is null) return Result.Failure<bool>("FTL Transporter not found.");

        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<bool>("Transporter name is required.");

        if (await transporterRepository.NameExistsAsync(name, command.Id, ct))
            return Result.Failure<bool>($"FTL Transporter '{name}' already exists.");

        transporter.Update(name, command.IsActive);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}
