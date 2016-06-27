using SXCore.Common.Interfaces;
using SXCore.Common.Values;

namespace Litskevich.Family.Domain.Events
{
    public class RegistrationRequestedEvent : IDomainEvent
    {
        public PersonTotalName Name { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string Comment { get; private set; }

        public RegistrationRequestedEvent(PersonTotalName name, string email, string phone, string comment)
        {
            this.Name = name;
            this.Email = email;
            this.Phone = phone;
            this.Comment = comment;
        }
    }
}
