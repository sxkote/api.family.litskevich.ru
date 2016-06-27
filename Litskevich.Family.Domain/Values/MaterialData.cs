using System;

namespace Litskevich.Family.Domain.Values
{
    public class MaterialData
    {
        public string Code { get; set; }
        public DateTimeOffset? Date { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
    }
}
