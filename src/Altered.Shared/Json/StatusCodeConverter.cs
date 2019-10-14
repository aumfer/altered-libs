using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Altered.Shared.Json
{
    internal sealed class StatusCodeConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;
        public override bool CanConvert(Type type) => type == typeof(StatusCode);

        public override void WriteJson(
            JsonWriter writer, object value, JsonSerializer serializer)
        {
            int statusCode = (StatusCode)value;
            writer.WriteValue(statusCode);
        }

        public override object ReadJson(
            JsonReader reader, Type type, object existingValue, JsonSerializer serializer)
        {
            var value = reader.ReadAsInt32();
            StatusCode statusCode = value ?? 0;
            return statusCode;
        }
    }
}
