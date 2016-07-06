using Litskevich.Family.Domain.Entities;
using SXCore.Common.Interfaces;

namespace Litskevich.Family.Domain.Events
{
    public class MaterialAddedToArticleEvent : IDomainEvent
    {
        public Article Article { get; private set; }
        public Material Material { get; private set; }

        public MaterialAddedToArticleEvent(Article article, Material material)
        {
            this.Article = article;
            this.Material = material;
        }
    }
}
