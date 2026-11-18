using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Entities.Ftl;

namespace Operations.Application.Operations.Ftl.CreateFtlTransporter;

public sealed class CreateFtlTransporterCommandHandler(
    IFtlTransporterRepository transporterRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<CreateFtlTransporterCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateFtlTransporterCommand command, CancellationToken ct)
    {
        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Guid>("Transporter name is required.");

        if (await transporterRepository.NameExistsAsync(name, null, ct))
            return Result.Failure<Guid>($"FTL Transporter '{name}' already exists.");

        var transporter = FtlTransporter.Create(name);
        await transporterRepository.AddAsync(transporter, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(transporter.Id);
    }
}
