using SXCore.Domain.Entities;
using System;

namespace Litskevich.Family.Domain.Entities
{
    public class ManagerRoleType : Types
    {
        private ManagerRoleType() { }

        static public ManagerRoleType Create(string name, string title = "")
        {
            return new ManagerRoleType()
            {
                Name = name,
                Title = String.IsNullOrEmpty(title) ? name : title
            };
        }
    }
}
