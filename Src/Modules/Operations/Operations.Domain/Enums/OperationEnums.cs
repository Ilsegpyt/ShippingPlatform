using System.ComponentModel;

namespace Operations.Domain.Enums;

public enum OperationType
{
    Import = 1,
    Export = 2
}
public enum OperationStatus
{
    Open = 1,
    Delivered = 2
}

public enum ClearanceType
{
    Bosla = 1,
    Cert = 2
}
public enum ContainerType
{
    [Description("20' Dry Standard")]
    DryStandard20 = 1,

    [Description("40' Dry Standard")]
    DryStandard40 = 2,

    [Description("40' Dry High")]
    DryHigh40 = 3,

    [Description("45' Dry High")]
    DryHigh45 = 4,

    [Description("20' Reefer Standard")]
    ReeferStandard20 = 5,

    [Description("40' Reefer High")]
    ReeferHigh40 = 6,

    [Description("20' Open Top")]
    OpenTop20 = 7,

    [Description("40' Open Top")]
    OpenTop40 = 8,

    [Description("40' Open Top High")]
    OpenTopHigh40 = 9,

    [Description("40' Flat Standard")]
    FlatStandard40 = 10,

    [Description("40' Flat High")]
    FlatHigh40 = 11,

    [Description("20' Flat")]
    Flat20 = 12,

    [Description("20' Tank")]
    Tank20 = 13,

    [Description("40' Tank")]
    Tank40 = 14
}