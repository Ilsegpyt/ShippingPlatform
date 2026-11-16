using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Operations.Application.Operations.Ftl.Common;

public static class FtlNormalizationHelper
{
    public static void NormalizeTripFields(Dictionary<string, string> fields)
    {
        if (fields == null) return;

        // Clean missDoc identical to FCL
        if (fields.TryGetValue("missDoc", out var md))
        {
            fields["missDoc"] = CleanDocValue(md);
        }

        // Clean car number
        if (fields.TryGetValue("car", out var carVal) && !string.IsNullOrWhiteSpace(carVal))
        {
            fields["car"] = carVal.Trim().ToUpperInvariant();
        }

        // Clean driver name to Title Case
        if (fields.TryGetValue("driver", out var driverVal) && !string.IsNullOrWhiteSpace(driverVal))
        {
            fields["driver"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(driverVal.Trim().ToLowerInvariant());
        }

        // Clean mobile number
        if (fields.TryGetValue("mob", out var mobVal) && !string.IsNullOrWhiteSpace(mobVal))
        {
            fields["mob"] = Regex.Replace(mobVal.Trim(), @"\s+", " ");
        }

        // Clean client
        if (fields.TryGetValue("client", out var cVal) && !string.IsNullOrWhiteSpace(cVal))
        {
            fields["client"] = cVal.Trim();
        }

        // Clean trucker / transporter
        if (fields.TryGetValue("trucker", out var trkVal) && !string.IsNullOrWhiteSpace(trkVal))
        {
            fields["trucker"] = trkVal.Trim();
        }

        // Clean destination & sites
        if (fields.TryGetValue("dest", out var destVal) && !string.IsNullOrWhiteSpace(destVal))
        {
            fields["dest"] = destVal.Trim();
        }
        if (fields.TryGetValue("aod", out var aodVal) && !string.IsNullOrWhiteSpace(aodVal))
        {
            fields["aod"] = aodVal.Trim();
        }
        if (fields.TryGetValue("pod", out var podVal) && !string.IsNullOrWhiteSpace(podVal))
        {
            fields["pod"] = podVal.Trim();
        }
        if (fields.TryGetValue("pol", out var polVal) && !string.IsNullOrWhiteSpace(polVal))
        {
            fields["pol"] = polVal.Trim();
        }
        if (fields.TryGetValue("pow", out var powVal) && !string.IsNullOrWhiteSpace(powVal))
        {
            fields["pow"] = powVal.Trim();
        }

        // Clean line / terminal
        if (fields.TryGetValue("line", out var lineVal) && !string.IsNullOrWhiteSpace(lineVal))
        {
            fields["line"] = lineVal.Trim();
        }

        // Clean BL / Booking / Cert
        if (fields.TryGetValue("bl", out var blVal) && !string.IsNullOrWhiteSpace(blVal))
        {
            fields["bl"] = blVal.Trim();
        }
        if (fields.TryGetValue("booking", out var bookVal) && !string.IsNullOrWhiteSpace(bookVal))
        {
            fields["booking"] = bookVal.Trim();
        }
        if (fields.TryGetValue("cert", out var certVal) && !string.IsNullOrWhiteSpace(certVal))
        {
            fields["cert"] = certVal.Trim();
        }
    }

    public static string CleanDocValue(string? val)
    {
        if (string.IsNullOrWhiteSpace(val)) return string.Empty;
        var s = val.Trim();
        if (Regex.IsMatch(s, @"^[-—_.\s]+$") ||
            s.Equals("none", StringComparison.OrdinalIgnoreCase) ||
            s.Equals("لا يوجد", StringComparison.OrdinalIgnoreCase) ||
            s.Equals("مكتمل", StringComparison.OrdinalIgnoreCase) ||
            s.Equals("لايوجد", StringComparison.OrdinalIgnoreCase) ||
            s.Equals("null", StringComparison.OrdinalIgnoreCase) ||
            s.Equals("undefined", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }
        return s;
    }
}
