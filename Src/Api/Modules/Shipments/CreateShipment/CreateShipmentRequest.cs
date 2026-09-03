namespace Api.Modules.Shipments.CreateShipment;

//public sealed record CreateShipmentRequest(
//    Guid ScheduleId,
//    int Quantity,
//    IReadOnlyCollection<IFormFile> DeclarationFiles
//    );

// الـ Minimal API model binding مع multipart/form-data والـ record اللي عندك بيحاول يبني الـ object عن طريق الـ constructor، ولما فشل في إيجاد DeclarationFiles
public sealed class CreateShipmentRequest
{
    public Guid ScheduleId { get; set; }

    public int Quantity { get; set; }

    public IFormFileCollection DeclarationFiles { get; set; } = null!;
}