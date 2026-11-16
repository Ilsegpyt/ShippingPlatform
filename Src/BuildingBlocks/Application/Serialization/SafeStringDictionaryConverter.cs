using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Application.Serialization;

/// <summary>
/// A JsonConverter for Dictionary&lt;string, string&gt; that safely converts any non-string primitive
/// (numbers, booleans, nulls) into a string representation instead of failing with a JsonException.
/// </summary>
public sealed class SafeStringDictionaryConverter : JsonConverter<Dictionary<string, string>>
{
    public static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new SafeStringDictionaryConverter() }
    };

    public override Dictionary<string, string>? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"Expected StartObject token for Dictionary<string, string>, got {reader.TokenType}.");
        }

        var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dictionary;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException($"Expected PropertyName token, got {reader.TokenType}.");
            }

            var propertyName = reader.GetString() ?? string.Empty;

            reader.Read();

            string value;
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    value = reader.GetString() ?? string.Empty;
                    break;
                case JsonTokenType.Number:
                    using (var doc = JsonDocument.ParseValue(ref reader))
                    {
                        value = doc.RootElement.GetRawText();
                    }
                    break;
                case JsonTokenType.True:
                    value = "true";
                    break;
                case JsonTokenType.False:
                    value = "false";
                    break;
                case JsonTokenType.Null:
                    value = string.Empty;
                    break;
                case JsonTokenType.StartObject:
                case JsonTokenType.StartArray:
                    using (var doc = JsonDocument.ParseValue(ref reader))
                    {
                        value = doc.RootElement.GetRawText();
                    }
                    break;
                default:
                    value = string.Empty;
                    break;
            }

            dictionary[propertyName] = value;
        }

        return dictionary;
    }

    public override void Write(
        Utf8JsonWriter writer,
        Dictionary<string, string> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var kvp in value)
        {
            writer.WriteString(kvp.Key, kvp.Value ?? string.Empty);
        }
        writer.WriteEndObject();
    }
}
