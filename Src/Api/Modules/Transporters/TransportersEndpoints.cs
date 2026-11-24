using Api.Modules.Transporters.ClientExceptions.AddClientException;
using Api.Modules.Transporters.Ports.AddOrUpdatePortCoverage;
using Api.Modules.Transporters.Ports.ApprovePortRates;
using Api.Modules.Transporters.Ports.GetPortCoverageById;
using Api.Modules.Transporters.Transporters.CreateTransporter;
using Api.Modules.Transporters.Transporters.GetTransporterById;
using Api.Modules.Transporters.Transporters.GetTransporters;
using Api.Modules.Transporters.Transporters.UpdateTransporter;

namespace Api.Modules.Transporters;

public static class TransportersEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        GetTransportersEndpoint.Map(app);
        GetTransporterByIdEndpoint.Map(app);
        CreateTransporterEndpoint.Map(app);
        UpdateTransporterEndpoint.Map(app);

        GetPortCoverageByIdEndpoint.Map(app);
        AddOrUpdatePortCoverageEndpoint.Map(app);
        ApprovePortRatesEndpoint.Map(app);

        AddClientExceptionEndpoint.Map(app);
    }
}
