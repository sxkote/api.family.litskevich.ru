using SXCore.Common.Entities;
using SXCore.Common.Services;
using System.Collections.Generic;

namespace Litskevich.Family.Domain.Entities
{
    public class Manager : Entity
    {
        public virtual Person Person { get; private set; }
        public string Login { get; private set; }
        public string Password { get; private set; }

        public virtual ICollection<ManagerRoleType> Roles { get; private set; }

        private Manager()
        {
            this.Roles = new List<ManagerRoleType>();
        }

        public void ChangePassword(string password)
        {
            this.Password = CommonService.HashPassword(password, 10);
        }

        public void ChangeRoles(params ManagerRoleType[] roles)
        {
            this.Roles.Clear();
            if (roles != null)
                foreach (var r in roles)
                    this.Roles.Add(r);
        }

        static public Manager Create(Person person, string login, string password)
        {
            var manager = new Manager()
            {
                Person = person,
                Login = login,
                Password = CommonService.HashPassword(password, 10)
            };

            return manager;
        }

        static public Manager Import(Person person, string login, string password)
        {
            var manager = new Manager()
            {
                Person = person,
                Login = login,
                Password = password
            };

            return manager;
        }
    }
}
