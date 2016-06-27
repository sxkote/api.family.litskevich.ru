using SXCore.Common.Values;
using System;

namespace Litskevich.Family.WebApi.Models.Requests
{
    public class UpdateArticleRequest
    {
        public string Title { get; set; }
        public DateTime? PeriodBegin { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public string Comment { get; set; }

        public long[] Persons { get; set; }
    }
}
