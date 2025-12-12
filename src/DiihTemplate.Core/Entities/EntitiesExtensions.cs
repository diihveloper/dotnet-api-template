using System.Text.Json;

namespace DiihTemplate.Core.Entities;

public static class EntitiesExtensions
{
    public static void SetMetadata<T>(this IHasMetadata entity, string key, T value)
    {
        entity.Metadata ??= new Dictionary<string, string>();

        if (value == null)
        {
            entity.Metadata[key] = null!;
            return;
        }

        // Verifica se o tipo é simples
        if (IsSimpleType(typeof(T)))
        {
            entity.Metadata[key] = value.ToString() ?? string.Empty;
        }
        else
        {
            entity.Metadata[key] = JsonSerializer.Serialize(value);
        }
    }

    public static T? GetMetadata<T>(this IHasMetadata entity, string key)
    {
        if (entity.Metadata == null) return default;

        if (!entity.Metadata.TryGetValue(key, out var rawValue))
            return default;

        if (IsSimpleType(typeof(T)))
        {
            return (T)Convert.ChangeType(rawValue, typeof(T));
        }

        return JsonSerializer.Deserialize<T>(rawValue);
    }

    private static bool IsSimpleType(Type type)
    {
        return
            type.IsPrimitive ||
            type.IsEnum ||
            type == typeof(string) ||
            type == typeof(decimal) ||
            type == typeof(DateTime) ||
            type == typeof(DateTimeOffset) ||
            type == typeof(Guid);
    }
}