using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.Ftl.AddFtlMaster;

public sealed record AddFtlMasterCommand(
    string ListName,
    string Value) : IRequest<Result<string>>;
