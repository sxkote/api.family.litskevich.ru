using Litskevich.Family.Domain.Entities;
using SXCore.Common.Interfaces;

namespace Litskevich.Family.Domain.Events
{
    public class MaterialDeletedEvent : IDomainEvent
    {
        public Article Article { get; private set; }
        public Material Material { get; private set; }

        public MaterialDeletedEvent(Article article, Material material)
        {
            this.Article = article;
            this.Material = material;
        }
    }
}
