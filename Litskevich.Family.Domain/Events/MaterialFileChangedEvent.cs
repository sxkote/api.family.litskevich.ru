using Litskevich.Family.Domain.Entities;
using SXCore.Common.Interfaces;

namespace Litskevich.Family.Domain.Events
{
    public class MaterialFileChangedEvent : IDomainEvent
    {
        public Material Material { get; private set; }

        public MaterialFileChangedEvent(Material material)
        {
            this.Material = material;
        }
    }
}
