using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiihTemplate.Core.Converters;

public class FlexibleIntConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            if (int.TryParse(str, out var value))
                return value;

            if (string.IsNullOrWhiteSpace(str))
                return 0;

            throw new JsonException($"Valor inválido para int: {str}");
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32();
        }

        throw new JsonException($"Token inesperado {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}