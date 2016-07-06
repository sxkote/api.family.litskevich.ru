using Newtonsoft.Json;
using System;

namespace Litskevich.Family.Infrastructure.Services.CloudConverter.Converters
{
    public class DateTimeConverter : JsonConverter
    {
        static private readonly DateTime BaseDate = new DateTime(1970, 1, 1);

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (reader.Value == null && objectType == typeof(DateTime?))
                    return (DateTime?)null;

                if (reader.Value is string)
                {
                    return Convert.ToDateTime(reader.Value);
                }
                else if (reader.Value is int || reader.Value is long)
                {
                    long seconds = (long)reader.Value;
                    return BaseDate.AddSeconds(seconds);
                }

                return null;
            }
            catch { return null; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var item = (DateTime?)value;

            if (item == null)
            {
                writer.WriteNull();
            }
            else
            {
                long seconds = (item.Value - BaseDate).Seconds;
                writer.WriteValue(seconds);
            }

            writer.Flush();
        }
    }
}
