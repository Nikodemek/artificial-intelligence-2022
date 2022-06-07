using System.Text.Json;

namespace MLP.Util;

public static class Serializer
{
    private static readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true
    };

    public static string Serialize<T>(T obj) where T : notnull
    {
        string data = JsonSerializer.Serialize<object>(obj, options);
        return data;
    }

    public static T Deserialize<T>(string data) where T : notnull
    {
        T obj = JsonSerializer.Deserialize<T>(data, options) ?? throw new JsonException("Json Deserialization went wrong!");
        return obj;
    }
}