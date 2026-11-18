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

public static class FtlTruckTypeExtensions
{
    public static FtlTruckType? ParseFtlTruckType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var v = value.Trim();

        // 1. Direct standard enum parse check
        if (Enum.TryParse<FtlTruckType>(v, true, out var directMatch))
            return directMatch;

        // 2. Normalize: remove spaces, underscores, hyphens, and convert Arabic numbers to English
        var cleaned = v.Replace(" ", "").Replace("_", "").Replace("-", "").ToLowerInvariant();
        cleaned = cleaned.Replace('٠', '0')
                         .Replace('١', '1')
                         .Replace('٢', '2')
                         .Replace('٣', '3')
                         .Replace('٤', '4')
                         .Replace('٥', '5')
                         .Replace('٦', '6')
                         .Replace('٧', '7')
                         .Replace('٨', '8')
                         .Replace('٩', '9');

        // Check enum parse on cleaned string
        if (Enum.TryParse<FtlTruckType>(cleaned, true, out var cleanedMatch))
            return cleanedMatch;

        // Numeric string
        if (int.TryParse(cleaned, out var num) && Enum.IsDefined(typeof(FtlTruckType), num))
            return (FtlTruckType)num;

        // Arabic / Text pattern matching
        if (cleaned.Contains("دباب") || cleaned.Contains("dabbaba"))
            return FtlTruckType.Dabbaba;

        if (cleaned.Contains("جامبو") || cleaned.Contains("jambo"))
        {
            if (cleaned.Contains("4"))
                return FtlTruckType.Jambo4m;
            if (cleaned.Contains("6"))
                return FtlTruckType.Jambo6m;
            return FtlTruckType.Jambo4m;
        }

        if (cleaned.Contains("تريل") || cleaned.Contains("trailer"))
        {
            if (cleaned.Contains("12"))
                return FtlTruckType.Trailer12m;
            if (cleaned.Contains("6"))
                return FtlTruckType.Trailer6m;
            return FtlTruckType.Trailer12m;
        }

        return null;
    }

    public static string? ToFrontendString(this FtlTruckType? type)
    {
        return type switch
        {
            FtlTruckType.Dabbaba => "Dabbaba",
            FtlTruckType.Jambo4m => "Jambo 4m",
            FtlTruckType.Jambo6m => "Jambo 6m",
            FtlTruckType.Trailer6m => "Trailer 6m",
            FtlTruckType.Trailer12m => "Trailer 12m",
            _ => null
        };
    }

    public static string ToFrontendString(this FtlTruckType type)
    {
        return type switch
        {
            FtlTruckType.Dabbaba => "Dabbaba",
            FtlTruckType.Jambo4m => "Jambo 4m",
            FtlTruckType.Jambo6m => "Jambo 6m",
            FtlTruckType.Trailer6m => "Trailer 6m",
            FtlTruckType.Trailer12m => "Trailer 12m",
            _ => type.ToString()
        };
    }
}

public enum FtlLoadStatus
{
    [Description("حمولة كاملة")]
    Full = 1,

    [Description("حمولة جزئية")]
    Partial = 2
}

