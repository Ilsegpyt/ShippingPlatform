using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.DeactivateFtlTransporter;

public sealed class DeactivateFtlTransporterCommandHandler(
    IFtlTransporterRepository transporterRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<DeactivateFtlTransporterCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeactivateFtlTransporterCommand command, CancellationToken ct)
    {
        var transporter = await transporterRepository.GetByIdAsync(command.Id, ct);
        if (transporter is null) return Result.Failure<bool>("FTL Transporter not found.");

        var userName = await currentUser.GetUserNameAsync(ct);
        transporter.MarkAsDeleted(currentUser.UserId, userName ?? "User", DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}
