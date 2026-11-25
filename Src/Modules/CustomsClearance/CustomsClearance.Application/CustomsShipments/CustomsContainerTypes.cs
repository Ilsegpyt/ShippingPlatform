namespace CustomsClearance.Application.CustomsShipments;

public static class CustomsContainerTypes
{
    public static readonly IReadOnlyList<string> All =
    [
        "DryStandard20",
        "DryStandard40",
        "DryHigh40",
        "DryHigh45",
        "ReeferStandard20",
        "ReeferHigh40",
        "OpenTop20",
        "OpenTop40",
        "OpenTopHigh40",
        "FlatStandard40",
        "FlatHigh40",
        "Flat20",
        "Tank20",
        "Tank40",
        "20",
        "40",
        "40 HC",
        "45 HC"
    ];

    public static bool IsValid(string? value) =>
        value is not null && All.Contains(value);
}
