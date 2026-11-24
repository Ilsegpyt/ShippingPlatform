using BuildingBlocks.Application;
using MediatR;
using Transporters.Application.Abstractions;
using Transporters.Application.Abstractions.Repositories;
using Transporters.Domain.Entities;

namespace Transporters.Application.Transporters.CreateTransporter;

public sealed class CreateTransporterCommandHandler(
    ITransporterRepository transporterRepository,
    ITransportersUnitOfWork transportersUnitOfWork)
    : IRequestHandler<CreateTransporterCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateTransporterCommand cmd,
        CancellationToken ct)
    {
        var transporter = new Transporter
        {
            Name = cmd.Name,
            ContactPerson = cmd.ContactPerson,
            Phone = cmd.Phone,
            Email = cmd.Email,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        transporterRepository.Add(transporter);

        await transportersUnitOfWork.SaveChangesAsync(ct);

        return Result.Success(transporter.Id);
    }
}
