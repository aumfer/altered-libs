using Altered.Shared.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Shared
{
    // a pattern for creating/using/reusing JsonSerializerSettings
    public static class AlteredJson
    {
        public static JsonSerializerSettings MakeDefault(this JsonSerializerSettings settings)
        {
            settings.Converters.Add(new StringEnumConverter());
            settings.Converters.Add(new StatusCodeConverter());
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return settings;
        }
        public static JsonSerializer MakeDefault(this JsonSerializer settings)
        {
            settings.Converters.Add(new StringEnumConverter());
            settings.Converters.Add(new StatusCodeConverter());
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return settings;
        }

        public static string SerializeObject<T>(T o) =>
            JsonConvert.SerializeObject(o, typeof(T), DefaultJsonSerializerSettings);

        public static JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings().MakeDefault();
        public static JsonSerializer DefaultJsonSerializer = new JsonSerializer().MakeDefault();
    }
}
