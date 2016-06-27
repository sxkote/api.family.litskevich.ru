using Litskevich.Family.Domain.Entities;
using SXCore.Common.Interfaces;

namespace Litskevich.Family.Domain.Events
{
    public class MaterialTransformEvent : IDomainEvent
    {
        public Material Material { get; private set; }
        public string Method { get; private set; }
        public string Argument { get; private set; }

        public MaterialTransformEvent(Material material, string method, string argument)
        {
            this.Material = material;
            this.Method = method;
            this.Argument = argument;
        }
    }
}
