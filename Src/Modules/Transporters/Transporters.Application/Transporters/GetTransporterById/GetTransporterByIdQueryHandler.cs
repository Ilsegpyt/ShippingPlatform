using BuildingBlocks.Application;
using MediatR;
using Transporters.Application.Abstractions.Repositories;
using Transporters.Domain.Entities;

namespace Transporters.Application.Transporters.GetTransporterById;

public sealed class GetTransporterByIdQueryHandler(
    ITransporterRepository transporterRepository)
    : IRequestHandler<
        GetTransporterByIdQuery,
        Result<Transporter>>
{
    public async Task<Result<Transporter>> Handle(
        GetTransporterByIdQuery query,
        CancellationToken ct)
    {
        var transporter =
            await transporterRepository.GetByIdAsync(
                query.Id,
                ct);

        if (transporter is null)
            return Result.Failure<Transporter>(
                "Transporter was not found.");

        return Result.Success(transporter);
    }
}
