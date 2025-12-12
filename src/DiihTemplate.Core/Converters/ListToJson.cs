using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiihTemplate.Core.Converters;

public class ListToJson<T> : ValueConverter<List<T>, string> where T : notnull
{
    public ListToJson() : base(
        x => JsonSerializer.Serialize(x, JsonSerializerOptions.Default),
        x => JsonSerializer.Deserialize<List<T>>(x, JsonSerializerOptions.Default) ??
             new List<T>())
    {
    }
}