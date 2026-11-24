using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StarSystemSimulator.Services
{
    public static class Serializer
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter() },
            IncludeFields = true
        };

        public static void SaveToFile<T>(T obj, string filePath)
        {
            string json = JsonSerializer.Serialize(obj, _options);
            File.WriteAllText(filePath, json);
        }

        public static T? LoadFromFile<T>(string filePath) where T : class
        {
            if (!File.Exists(filePath))
                return null;

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json, _options);
        }
    }
}
