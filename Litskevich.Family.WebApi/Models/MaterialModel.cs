using System;

namespace Litskevich.Family.WebApi.Models
{
    public class MaterialModel
    {
        public long ID { get; set; }
        public string Url { get; set; }
        public string FileName { get; set; }
        public string Type { get; set; }
        public DateTime? Date { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
    }
}
