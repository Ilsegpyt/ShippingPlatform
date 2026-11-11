using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.GetFtlMaster;

public sealed record FtlMasterDto(
    IReadOnlyList<string> Client,
    IReadOnlyList<string> Line,
    IReadOnlyList<string> Dest,
    IReadOnlyList<string> Pod,
    IReadOnlyList<string> Trucker,
    IReadOnlyList<string> Driver,
    IReadOnlyList<string> Cntr,
    IReadOnlyList<string> TruckType,
    IReadOnlyList<string> ClrType,
    IReadOnlyList<string> Customs,
    IReadOnlyList<string> Status);

public sealed record GetFtlMasterQuery : IRequest<Result<FtlMasterDto>>;

