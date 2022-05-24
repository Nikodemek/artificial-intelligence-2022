using System.Text;
using System.Text.Json;

namespace MLP.Util;

public static class Serializer
{
    public static string Serialize<T>(T obj)
    {
        string data = JsonSerializer.Serialize(obj);
        return data;
    }

    public static T Deserialize<T>(string data)
    {
        T obj = JsonSerializer.Deserialize<T>(data) ?? throw new JsonException("Json Deserialization went wrong!");
        return obj;
    }
}