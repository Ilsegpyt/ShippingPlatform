namespace Operations.Application.Common;

/// <summary>
/// Creates normalized duplicate keys for completed Import/Export identifiers.
/// </summary>
public static class OperationDuplicateCheckKey
{
    public static string? Create(
        string? shipmentNumber,
        string? documentNumber,
        string? certificateNumber)
    {
        if (string.IsNullOrWhiteSpace(shipmentNumber) ||
            string.IsNullOrWhiteSpace(documentNumber) ||
            string.IsNullOrWhiteSpace(certificateNumber))
        {
            return null;
        }

        return string.Join(
            '|',
            Normalize(shipmentNumber),
            Normalize(documentNumber),
            Normalize(certificateNumber));
    }

    public static string Normalize(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var builder = new System.Text.StringBuilder(value.Length);

        foreach (var character in value.Trim())
        {
            if (character is >= '\u0660' and <= '\u0669')
            {
                builder.Append((char)('0' + character - '\u0660'));
            }
            else if (character is >= '\u06F0' and <= '\u06F9')
            {
                builder.Append((char)('0' + character - '\u06F0'));
            }
            else if (!char.IsWhiteSpace(character))
            {
                builder.Append(char.ToUpperInvariant(character));
            }
        }

        return builder.ToString();
    }
}