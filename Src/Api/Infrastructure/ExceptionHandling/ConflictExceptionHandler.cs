using BuildingBlocks.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Api.Infrastructure.ExceptionHandling;

public sealed class ConflictExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ConflictException conflictException)
            return false;

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

        await httpContext.Response.WriteAsJsonAsync(
            new
            {
                title = "Conflict",
                detail = conflictException.Message,
                status = StatusCodes.Status409Conflict
            },
            cancellationToken);

        return true;
    }
}