using BuildingBlocks.Application;
using Customers.Contracts;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.GetFtlMaster;

public sealed class GetFtlMasterQueryHandler(
    ICustomerQueries customerQueries,
    IDestinationRepository destinationRepository,
    IFtlAirportRepository airportRepository,
    IFtlTerminalRepository terminalRepository,
    IFtlTransporterRepository transporterRepository,
    IFtlDriverRepository driverRepository,
    IFtlVehicleRepository vehicleRepository)
    : IRequestHandler<GetFtlMasterQuery, Result<FtlMasterDto>>
{
    public async Task<Result<FtlMasterDto>> Handle(GetFtlMasterQuery query, CancellationToken ct)
    {
        List<string> clients = [];
        List<string> dests = [];
        List<string> pods = [];
        List<string> lines = [];
        List<string> truckers = [];
        List<string> drivers = [];
        List<string> cntrs = [];

        try
        {
            var customers = await customerQueries.GetAllForOperationsAsync(ct);
            clients = customers.Select(c => c.CompanyName).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(s => s).ToList();
        }
        catch { /* fallback */ }

        try
        {
            var destinations = await destinationRepository.GetAllAsync(false, ct);
            dests = destinations.Select(d => d.Name).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(s => s).ToList();
        }
        catch { /* fallback */ }

        try
        {
            var airports = await airportRepository.GetAllAsync(false, ct);
            pods = airports.Select(a => !string.IsNullOrWhiteSpace(a.Code) ? a.Code : a.Name).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(s => s).ToList();
        }
        catch { /* fallback */ }

        try
        {
            var terminals = await terminalRepository.GetAllAsync(false, ct);
            lines = terminals.Select(t => t.Name).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(s => s).ToList();
        }
        catch { /* fallback */ }

        try
        {
            var transporters = await transporterRepository.GetAllAsync(false, ct);
            truckers = transporters.Select(t => t.Name).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(s => s).ToList();
        }
        catch { /* fallback */ }

        try
        {
            var drvs = await driverRepository.GetAllAsync(false, ct);
            drivers = drvs.Select(d => d.Name).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(s => s).ToList();
        }
        catch { /* fallback */ }

        try
        {
            var vehs = await vehicleRepository.GetAllAsync(false, ct);
            cntrs = vehs.Select(v => v.PlateNumber).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(s => s).ToList();
        }
        catch { /* fallback */ }

        if (pods.Count == 0) pods = ["CAI", "HBE", "LXR", "ASW", "HRG", "SSH"];
        if (lines.Count == 0) lines = ["CACC Terminal", "Cairo Cargo Village", "EgyptAir Cargo", "EAS Cargo"];
        if (truckers.Count == 0) truckers = ["EURO", "ALRihab", "ALDAMNHORY", "TAQA", "Ranin"];
        if (dests.Count == 0) dests = ["10th of Ramadan", "6th of October", "Al Obour City", "Borg El Arab", "Sadat City"];

        var truckTypes = new List<string> { "دبابة", "جامبو 4م", "جامبو 6م", "تريلا 6م", "تريلا 12م" };
        var clrTypes = new List<string> { "CERT", "BOSLA" };
        var customs = new List<string> { "Cairo Airport Customs", "Cargo Village Customs", "Free Zone Customs" };
        var statuses = new List<string> { "Open", "Delivered" };

        var result = new FtlMasterDto(
            clients,
            lines,
            dests,
            pods,
            truckers,
            drivers,
            cntrs,
            truckTypes,
            clrTypes,
            customs,
            statuses);

        return Result.Success(result);
    }
}

