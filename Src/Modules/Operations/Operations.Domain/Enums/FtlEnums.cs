using System.ComponentModel;

namespace Operations.Domain.Enums;

public enum FtlTruckType
{
    [Description("دبابة")]
    Dabbaba = 1,

    [Description("جامبو 4م")]
    Jambo4m = 2,

    [Description("جامبو 6م")]
    Jambo6m = 3,

    [Description("تريلا 6م")]
    Trailer6m = 4,

    [Description("تريلا 12م")]
    Trailer12m = 5
}

public enum FtlLoadStatus
{
    [Description("حمولة كاملة")]
    Full = 1,

    [Description("حمولة جزئية")]
    Partial = 2
}

