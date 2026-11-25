using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.GetFtlFilterOptions;

public sealed record GetFtlFilterOptionsQuery(
    string? OperationType = null,
    string? Status = null,
    bool OnlyActive = false,
    bool IsDeleted = false) : IRequest<Result<FtlFilterOptionsDto>>;
