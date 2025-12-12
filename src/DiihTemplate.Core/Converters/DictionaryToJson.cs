using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiihTemplate.Core.Converters;

public class DictionaryToJson<TKey, TValue> : ValueConverter<Dictionary<TKey, TValue>, string> where TKey : notnull
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.General)
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public DictionaryToJson() : base(
        x => JsonSerializer.Serialize(x, Options),
        x => JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(x, Options) ??
             new Dictionary<TKey, TValue>()
    )
    {
    }
}