using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RFETest.Utils
{
    public static class SerializationUtils
    {
        public static JsonSerializerOptions Options = new JsonSerializerOptions();

        static SerializationUtils()
        {
            Options.Converters.Add(new JsonStringEnumConverter());
            Options.PropertyNameCaseInsensitive = true;
        }
    }
}
