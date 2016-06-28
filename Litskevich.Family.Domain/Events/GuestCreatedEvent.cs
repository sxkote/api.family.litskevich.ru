using SXCore.Common.Interfaces;
using SXCore.Common.Values;

namespace Litskevich.Family.Domain.Events
{
    public class GuestCreatedEvent : IDomainEvent
    {
        public PersonName Name { get; private set; }
        public string Email { get; private set; }
        public string Login { get; private set; }
        public string Password { get; private set; }

        public GuestCreatedEvent(PersonName name, string email, string login, string password)
        {
            this.Name = name;
            this.Email = email;
            this.Login = login;
            this.Password = password;
        }
    }
}
