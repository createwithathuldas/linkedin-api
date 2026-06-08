using System.Reflection;
using System.Text.Json;
using linkedin_api.Data;
using linkedin_api.Models;
using Microsoft.EntityFrameworkCore;

namespace linkedin_api.Services;

public interface IApiEntityService
{
    T Create<T>(Dictionary<string, object?> values) where T : EntityBase, new();
    void Apply<T>(T entity, Dictionary<string, object?> values) where T : EntityBase;
}

public class ApiEntityService : IApiEntityService
{
    public T Create<T>(Dictionary<string, object?> values) where T : EntityBase, new()
    {
        var entity = new T();
        Apply(entity, values);
        return entity;
    }

    public void Apply<T>(T entity, Dictionary<string, object?> values) where T : EntityBase
    {
        var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.CanWrite && p.Name is not nameof(EntityBase.Id) and not nameof(EntityBase.CreatedAt))
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
        foreach (var item in values)
        {
            if (!props.TryGetValue(item.Key, out var prop)) continue;
            var value = ConvertValue(item.Value, prop.PropertyType);
            prop.SetValue(entity, value);
        }
        entity.UpdatedAt = DateTime.UtcNow;
    }

    private static object? ConvertValue(object? value, Type target)
    {
        if (value is null) return null;
        var isNullable = Nullable.GetUnderlyingType(target) != null || !target.IsValueType;
        var type = Nullable.GetUnderlyingType(target) ?? target;

        if (value is JsonElement json)
        {
            if (json.ValueKind == JsonValueKind.Null) return null;
            if (json.ValueKind == JsonValueKind.String)
            {
                var str = json.GetString();
                if (type == typeof(string)) return str;
                if (string.IsNullOrWhiteSpace(str))
                {
                    if (isNullable) return null;
                }
                if (type == typeof(DateTime))
                {
                    if (DateTime.TryParse(str, out var dt)) return dt;
                    if (isNullable) return null;
                }
            }
            if (type.IsEnum) return json.ValueKind == JsonValueKind.Number ? Enum.ToObject(type, json.GetInt32()) : Enum.Parse(type, json.GetString() ?? "", true);
            return JsonSerializer.Deserialize(json.GetRawText(), type);
        }

        var valStr = value.ToString();
        if (type == typeof(string)) return valStr;
        if (string.IsNullOrWhiteSpace(valStr))
        {
            if (isNullable) return null;
        }
        if (type == typeof(DateTime))
        {
            if (DateTime.TryParse(valStr, out var dt)) return dt;
            if (isNullable) return null;
        }
        if (type.IsEnum) return Enum.Parse(type, valStr ?? "", true);
        return Convert.ChangeType(value, type);
    }
}

public static class QueryableExtensions
{
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int page = 1, int pageSize = 20)
        => query.Skip(Math.Max(0, page - 1) * Math.Clamp(pageSize, 1, 100)).Take(Math.Clamp(pageSize, 1, 100));
}
