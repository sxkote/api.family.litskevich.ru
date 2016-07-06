using Litskevich.Family.Infrastructure.Services.CloudConverter.Options;
using Newtonsoft.Json;
using System;

namespace Litskevich.Family.Infrastructure.Services.CloudConverter.Converters
{
    public class InputMethodConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(InputMethod);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var item = (InputMethod)value;

            switch (item)
            {
                case InputMethod.None:
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
