using Litskevich.Family.Domain.Entities;
using SXCore.Common.Interfaces;

namespace Litskevich.Family.Domain.Events
{
    public class MaterialInfoChangedEvent : IDomainEvent
    {
        public Material Material { get; private set; }

        public MaterialInfoChangedEvent(Material material)
        {
            this.Material = material;
        }
    }
}
