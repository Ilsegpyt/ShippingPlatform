using BuildingBlocks.Application;
using MediatR;
using Transporters.Application.Abstractions;
using Transporters.Application.Abstractions.Repositories;

namespace Transporters.Application.Transporters.UpdateTransporter;

public sealed class UpdateTransporterCommandHandler(
    ITransporterRepository transporterRepository,
    ITransportersUnitOfWork transportersUnitOfWork)
    : IRequestHandler<UpdateTransporterCommand, Result>
{
    public async Task<Result> Handle(
        UpdateTransporterCommand cmd,
        CancellationToken ct)
    {
        var transporter = await transporterRepository.GetForUpdateAsync(cmd.Id, ct);

        if (transporter is null)
            return Result.Failure("Transporter was not found.");

        transporter.Name = cmd.Name;
        transporter.ContactPerson = cmd.ContactPerson;
        transporter.Phone = cmd.Phone;
        transporter.Email = cmd.Email;
        transporter.IsActive = cmd.IsActive;

        await transportersUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
