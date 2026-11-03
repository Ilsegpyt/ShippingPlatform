using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.DeactivateTransporter;

public sealed class DeactivateTransporterCommandHandler(
    ITransporterRepository transporterRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<DeactivateTransporterCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeactivateTransporterCommand command, CancellationToken ct)
    {
        var transporter = await transporterRepository.GetByIdAsync(command.Id, ct);
        if (transporter is null) return Result.Failure<bool>("Transporter not found.");

        var userName = await currentUser.GetUserNameAsync(ct);
        transporter.MarkAsDeleted(currentUser.UserId, userName ?? "User", DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
