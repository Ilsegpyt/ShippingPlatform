namespace Shipments.Application.Shipments.CreateShipment;

public sealed record DeclarationFileInput(
    string FileName,
    Stream Content);