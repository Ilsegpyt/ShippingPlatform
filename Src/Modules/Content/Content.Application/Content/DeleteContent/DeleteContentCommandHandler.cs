using BuildingBlocks.Application;
using Content.Application.Abstractions;
using MediatR;

namespace Content.Application.Content.DeleteContent;

public sealed class DeleteContentCommandHandler(
    IContentRepository contentRepository,
    IContentUnitOfWork unitOfWork)
    : IRequestHandler<DeleteContentCommand, Result>
{
    public async Task<Result> Handle(
        DeleteContentCommand command,
        CancellationToken ct)
    {
        var content = await contentRepository.GetByIdAsync(
            command.Id,
            ct);

        if (content is null)
            return Result.Failure("Content not found.");

        contentRepository.Delete(content);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}