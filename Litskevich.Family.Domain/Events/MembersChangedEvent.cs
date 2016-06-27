using Litskevich.Family.Domain.Entities;
using SXCore.Common.Interfaces;
using System.Collections.Generic;

namespace Litskevich.Family.Domain.Events
{
    public class MembersChangedEvent : IDomainEvent
    {
        public Article Article { get; private set; }
        public IEnumerable<Person> Members { get; private set; }

        public MembersChangedEvent(Article article, IEnumerable<Person> members)
        {
            this.Article = article;
            this.Members = members;
        }
    }
}
