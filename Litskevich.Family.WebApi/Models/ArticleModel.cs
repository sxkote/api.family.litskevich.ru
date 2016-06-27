using SXCore.Common.Values;
using System;
using System.Collections.Generic;

namespace Litskevich.Family.WebApi.Models
{
    public class ArticleModel
    {
        public long ID { get; set; }

        public AuthorModel Author { get; set; }
        public DateTimeOffset Date { get; set; }

        public string Title { get; set; }
        public DateTime? PeriodBegin { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public string Comment { get; set; }

        public ICollection<MaterialModel> Materials { get; set; }
        public ICollection<MemberModel> Members { get; set; }
    }
}
