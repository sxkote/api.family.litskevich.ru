using Litskevich.Family.Infrastructure.Services.CloudConverter.Options;
using Newtonsoft.Json;
using System;

namespace Litskevich.Family.Infrastructure.Services.CloudConverter.Converters
{
    public class OutputStorageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(OutputStorage);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var item = (OutputStorage)value;

            switch (item)
            {
                case OutputStorage.None:
                    writer.WriteUndefined();
                    break;
                default:
                    writer.WriteValue(item.ToString().ToLower());
                    break;
            }

            writer.Flush();
        }
    }
}
