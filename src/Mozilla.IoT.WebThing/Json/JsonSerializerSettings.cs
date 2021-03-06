using System.Text.Json;

namespace Mozilla.IoT.WebThing.Json
{
    public class JsonSerializerSettings : IJsonSerializerSettings
    {
        public JsonSerializerSettings(JsonSerializerOptions options)
        {
            Options = options;
        }

        public JsonSerializerOptions Options { get; }

        public bool IgnoreNullValues => Options.IgnoreNullValues;
        public bool WriteIndented => Options.WriteIndented;
    }
}
