using Newtonsoft.Json;
using System;

namespace Litskevich.Family.Infrastructure.Services.CloudConverter.Responses
{
    public class StatusResponse : BaseResponse
    {
        public const string StepFinished = "finished";

        public string Id { get; set; }
        public string Url { get; set; }
        public double Percent { get; set; }
        public string Message { get; set; }
        public string Step { get; set; }
        public string Group { get; set; }
        //public string Path { get; set; }
        public double Minutes { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? Expire { get; set; }
        public DateTime? EndTime { get; set; }

        public StatusFileInfo Input { get; set; }
        public StatusFileInfo Output { get; set; }
        public StatusConverter Converter { get; set; }

        public bool IsFinished { get { return this.Step.Equals(StepFinished, StringComparison.InvariantCultureIgnoreCase); } }
    }

    public class StatusConverter
    {
        public string Format { get; set; }
        public string Mode { get; set; }
        public string Type { get; set; }
        public object Options { get; set; }
    }

    public class StatusFileInfo
    {
        [JsonProperty(PropertyName = "Ext")]
        public string Extension { get; set; }
        public string Filename { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public string Url { get; set; }
    }
}
