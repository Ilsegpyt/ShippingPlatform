namespace Operations.Domain.Common;

public static class IdentifierNormalizer
{
    public static string? Normalize(string? value)
    {
        if (value is null)
            return null;

        var builder = new System.Text.StringBuilder(value.Length);

        foreach (var character in value)
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