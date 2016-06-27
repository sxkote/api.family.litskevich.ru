using System;

namespace Litskevich.Family.WebApi.Models.Requests
{
    public class UpdateMaterialRequest
    {
        public DateTime? Date { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
    }
}
