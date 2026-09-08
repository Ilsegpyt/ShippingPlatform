using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using MediatR;

namespace Customers.Application.Commands.DeleteSearchHistories;

public sealed class DeleteSearchHistoriesCommandHandler(
    ISearchHistoryRepository searchHistoryRepository,
    ICustomersUnitOfWork unitOfWork)
    : IRequestHandler<DeleteSearchHistoriesCommand, Result<DeleteSearchHistoriesResponse>>
{
    public async Task<Result<DeleteSearchHistoriesResponse>> Handle(
        DeleteSearchHistoriesCommand command,
        CancellationToken ct)
    {
        var searchHistories = await searchHistoryRepository.GetByIdsAsync(
            command.SearchHistoryIds,
            ct);

        if (searchHistories.Count == 0)
        {
            return Result.Failure<DeleteSearchHistoriesResponse>(
                "No search history records were found.");
        }

        searchHistoryRepository.RemoveRange(searchHistories);

        await unitOfWork.SaveChangesAsync(ct);

        return new DeleteSearchHistoriesResponse(
            searchHistories.Count);
    }
}