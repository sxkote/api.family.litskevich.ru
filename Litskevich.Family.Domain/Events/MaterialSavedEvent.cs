using Litskevich.Family.Domain.Entities;
using SXCore.Common.Interfaces;

namespace Litskevich.Family.Domain.Events
{
    public class MaterialSavedEvent : IDomainEvent
    {
        public Material Material { get; private set; }

        public MaterialSavedEvent(Material material)
        {
            this.Material = material;
        }


    }
}
