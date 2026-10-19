using System.Text;

namespace Operations.Application.Common;

/// <summary>Builds the globally unique FCL approval key: CNTR|M/BL|BOOKING.</summary>
public static class FclApprovalKey
{
    public static string Create(string containerNumber, string mblNumber, string bookingNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(containerNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(mblNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(bookingNumber);

        return string.Join('|', Normalize(containerNumber), Normalize(mblNumber), Normalize(bookingNumber));
    }

    public static string Normalize(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var character in value.Trim())
        {
            if (character is >= '\u0660' and <= '\u0669')
                builder.Append((char)('0' + character - '\u0660'));
            else if (character is >= '\u06F0' and <= '\u06F9')
                builder.Append((char)('0' + character - '\u06F0'));
            else if (!char.IsWhiteSpace(character))
                builder.Append(char.ToUpperInvariant(character));
        }

        return builder.ToString();
    }
}
