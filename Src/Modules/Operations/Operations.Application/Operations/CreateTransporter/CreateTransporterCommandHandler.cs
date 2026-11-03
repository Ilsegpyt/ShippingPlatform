using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Application.Operations.CreateTransporter;

public sealed class CreateTransporterCommandHandler(
    ITransporterRepository transporterRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<CreateTransporterCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTransporterCommand command, CancellationToken ct)
    {
        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Guid>("Transporter name is required.");

        if (await transporterRepository.TransporterNameExistsAsync(name, null, ct))
            return Result.Failure<Guid>($"Transporter '{name}' already exists.");

        var transporter = Transporter.Create(name);
        await transporterRepository.AddAsync(transporter, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(transporter.Id);
    }
}
