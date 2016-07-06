using Litskevich.Family.Infrastructure.Services.CloudConverter.Options;
using Newtonsoft.Json;
using System;

namespace Litskevich.Family.Infrastructure.Services.CloudConverter.Converters
{
    public class DownloadMethodConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DownloadMethod);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var item = (DownloadMethod)value;

            switch (item)
            {
                case DownloadMethod.None:
                    writer.WriteUndefined();
                    break;
                case DownloadMethod.True:
                    writer.WriteValue(true);
                    break;
                case DownloadMethod.False:
                    writer.WriteValue(false);
                    break;
                default:
                    writer.WriteValue(item.ToString().ToLower());
                    break;
            }

            writer.Flush();
        }
    }
}
