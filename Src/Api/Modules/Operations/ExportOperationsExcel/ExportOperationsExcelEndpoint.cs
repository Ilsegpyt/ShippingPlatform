

using System.Globalization;

using Identity.Domain.ValueObjects;

using Identity.Infrastructure.Authorization;

using Customers.Contracts;

using Operations.Application.Abstractions;

using Operations.Application.Operations.SearchOperations;



namespace Api.Modules.Operations.ExportOperationsExcel;



public sealed record ExportOperationsExcelRequest(

    AdvancedOperationsSearchRequest? Search = null,

    IReadOnlyList<Guid>? OperationIds = null,

    DateTime? CreatedFromUtc = null,

    DateTime? CreatedToUtc = null,

    IReadOnlyList<string>? Columns = null);



public static class ExportOperationsExcelEndpoint

{

    private static readonly IReadOnlyDictionary<string, string> Headers =

        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)

        {

            ["OperationId"] = "Operation ID",

            ["CreatedAtUtc"] = "Created At (UTC)",

            ["CreatedByName"] = "Created By",

            ["ClientName"] = "Customer Name",

            ["ShippingLine"] = "Shipping Line",

            ["OperationType"] = "Operation Type",

            ["Status"] = "Status",

            ["Source"] = "Source",

            ["ShipmentNumber"] = "Shipment Number",

            ["CertificateNumber"] = "Certificate Number",

            ["InvoicesReceivedDate"] = "Invoices Received Date",

            ["DeliveredAtUtc"] = "Delivered At (UTC)",

            ["NumberOfContainers"] = "Number Of Containers",

            ["MBLNumber"] = "M/BL",

            ["POD"] = "POD",

            ["RequiredOffloadingDate"] = "Required Offloading Date",

            ["RequiredOffloadingTime"] = "Required Offloading Time",

            ["ClearanceType"] = "Clearance Type",

            ["BookingNumber"] = "Booking Number",

            ["POL"] = "POL",

            ["POW"] = "POW",

            ["CutOffDate"] = "Cut Off Date",

            ["Customs"] = "Customs",

            ["ContainerOrdinal"] = "Container #",

            ["ContainerNumber"] = "Container Number",

            ["ContainerType"] = "Container Type",

            ["Destination"] = "Destination",

            ["FreeTimeTill"] = "Free Time Till",

            ["RequiredLoadingDate"] = "Required Loading Date",

            ["RequiredLoadingTime"] = "Required Loading Time",

            ["ActualArrivalDate"] = "Actual Arrival Date",

            ["ActualArrivalTime"] = "Actual Arrival Time",

            ["GateOutDate"] = "Gate Out Date",

            ["GateOutTime"] = "Gate Out Time",

            ["MissingDocs"] = "Missing Docs",

            ["MissingDocsReceivedDate"] = "Missing Docs Received Date",

            ["CheckOutFromThePort"] = "Check Out From Port",

            ["Transporter"] = "Transporter",

            ["DriverNumber"] = "Driver Number",

            ["DriverName"] = "Driver Name",

            ["DriverMobile"] = "Driver Mobile",

            ["VehicleNumber"] = "Vehicle Number"

        };



    public static void Map(IEndpointRouteBuilder app)

    {

        app.MapPost(

            "/api/operations/export-excel",

            async (

                ExportOperationsExcelRequest request,

                IOperationRepository repository,

                ICustomerQueries customerQueries,

                CancellationToken ct) =>

            {

                if (request.CreatedFromUtc.HasValue &&

                    request.CreatedToUtc.HasValue &&

                    request.CreatedFromUtc > request.CreatedToUtc)

                {

                    return Results.BadRequest(new

                    {

                        Error = "CreatedFromUtc must be earlier than or equal to CreatedToUtc."

                    });

                }



                if (request.OperationIds is { Count: 0 })

                {

                    return Results.BadRequest(new

                    {

                        Error = "OperationIds cannot be empty. Omit it to export all matching operations."

                    });

                }



                if (request.OperationIds is { Count: > 5000 })

                {

                    return Results.BadRequest(new

                    {

                        Error = "A maximum of 5000 operation IDs is allowed."

                    });

                }



                var columns = request.Columns is { Count: > 0 }

                    ? request.Columns

                        .Distinct(StringComparer.OrdinalIgnoreCase)

                        .ToList()

                    : Headers.Keys.ToList();



                var invalid = columns

                    .Where(column => !Headers.ContainsKey(column))

                    .ToArray();



                if (invalid.Length > 0)

                {

                    return Results.BadRequest(new

                    {

                        Error = "Unsupported export columns.",

                        Columns = invalid

                    });

                }



                if (columns.Count == 0)

                {

                    return Results.BadRequest(new

                    {

                        Error = "Select at least one column."

                    });

                }



                var search = request.Search ??

                    new AdvancedOperationsSearchRequest(

                        null,

                        "CreatedAtUtc",

                        "Desc",

                        null,

                        null,

                        1,

                        50);



                if (search.Rules is { Count: > 20 })

                {

                    return Results.BadRequest(new

                    {

                        Error = "A maximum of 20 search rules is allowed."

                    });

                }



                try

                {

                    var operations = await repository.SearchForExportAsync(

                        search,

                        request.OperationIds,

                        request.CreatedFromUtc,

                        request.CreatedToUtc,

                        ct);



                    var customerIds = operations

                        .Select(operation => operation.ClientId)

                        .Distinct()

                        .ToArray();

                    var customers = await customerQueries.GetByIdsAsync(customerIds, ct);

                    var clientNames = customers.ToDictionary(

                        customer => customer.CustomerId,

                        customer => customer.CompanyName);



                    var rows = new List<IReadOnlyList<string?>>

                    {

                        columns.Select(column => Headers[column]).ToArray()

                    };



                    foreach (var operation in operations)

                    {

                        var containers = operation.Containers

                            .OrderBy(container => container.Ordinal)

                            .ToList();



                        if (containers.Count == 0)

                        {

                            rows.Add(

                                columns.Select(column =>

                                    GetValue(column, operation, null, clientNames))

                                .ToArray());



                            continue;

                        }



                        foreach (var container in containers)

                        {

                            rows.Add(

                                columns.Select(column =>

                                    GetValue(column, operation, container, clientNames))

                                .ToArray());

                        }

                    }



                    var bytes = MinimalXlsx.Create(rows);



                    var fileName =

                        $"operations-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx";



                    return Results.File(

                        bytes,

                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

                        fileName);

                }

                catch (ArgumentException ex)

                {

                    return Results.BadRequest(new

                    {

                        Error = ex.Message

                    });

                }

            })

            .RequirePermission(PermissionCatalog.OperationsView);

    }



    private static string? GetValue(

        string key,

        global::Operations.Domain.Entities.Operation o,

        global::Operations.Domain.Entities.OperationContainer? c,

        IReadOnlyDictionary<Guid, string?> clientNames)

    {

        object? value = key switch

        {

            "OperationId" => o.Id,

            "CreatedAtUtc" => o.CreatedAtUtc,

            "CreatedByName" => o.CreatedByName,

            "ClientName" => clientNames.GetValueOrDefault(o.ClientId),

            "ShippingLine" => o.ShippingLine?.Name,

            "OperationType" => o.OperationType,

            "Status" => o.Status,

            "Source" => o.Source,

            "ShipmentNumber" => o.ShipmentNumber,

            "CertificateNumber" => o.CertificateNumber,

            "InvoicesReceivedDate" => o.InvoicesReceivedDate,

            "DeliveredAtUtc" => o.DeliveredAtUtc,

            "NumberOfContainers" => o.NumberOfContainers,



            "MBLNumber" => o.ImportDetails?.MBLNumber,

            "POD" => o.ImportDetails?.POD?.Name,

            "RequiredOffloadingDate" =>

                o.ImportDetails?.RequiredOffloadingDate,

            "RequiredOffloadingTime" =>

                o.ImportDetails?.RequiredOffloadingTime,



            "ClearanceType" => o.ExportDetails?.ClearanceType,

            "BookingNumber" => o.ExportDetails?.BookingNumber,

            "POL" => o.ExportDetails?.POL?.Name,

            "POW" => o.ExportDetails?.POW?.Name,

            "CutOffDate" => o.ExportDetails?.CutOffDate,

            "Customs" => o.ExportDetails?.Customs,



            "ContainerOrdinal" => c?.Ordinal,

            "ContainerNumber" => c?.ContainerNumber,

            "ContainerType" => c?.ContainerType,

            "Destination" => c?.Destination,

            "FreeTimeTill" => c?.FreeTimeTill,

            "RequiredLoadingDate" => c?.RequiredLoadingDate,

            "RequiredLoadingTime" => c?.RequiredLoadingTime,

            "ActualArrivalDate" => c?.ActualArrivalDate,

            "ActualArrivalTime" => c?.ActualArrivalTime,

            "GateOutDate" => c?.GateOutDate,

            "GateOutTime" => c?.GateOutTime,

            "MissingDocs" => c?.MissingDocs,

            "MissingDocsReceivedDate" => c?.MissingDocsReceivedDate,

            "CheckOutFromThePort" => c?.CheckOutFromThePort,

            "Transporter" => c?.Transporter?.Name,

            "DriverNumber" => c?.Driver?.DriverNumber,

            "DriverName" => c?.Driver?.Name,

            "DriverMobile" => c?.Driver?.MobileNumber,

            "VehicleNumber" => c?.Vehicle?.CarNumber,



            _ => null

        };



        return value switch

        {

            null => null,

            DateTime dt =>

                dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),

            DateOnly date =>

                date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),

            TimeOnly time =>

                time.ToString("HH:mm:ss", CultureInfo.InvariantCulture),

            _ => Convert.ToString(value, CultureInfo.InvariantCulture)

        };

    }

}
