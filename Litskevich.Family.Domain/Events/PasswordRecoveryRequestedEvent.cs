using SXCore.Common.Interfaces;
using SXCore.Common.Values;

namespace Litskevich.Family.Domain.Events
{
    public class PasswordRecoveryRequestedEvent : IDomainEvent
    {
        public string Code { get; set; }
        public PersonName Name { get; set; }
        public string Email { get; set; }

        public PasswordRecoveryRequestedEvent(string code, PersonName name, string email)
        {
            this.Code = code;
            this.Name = name;
            this.Email = email;
        }
    }
}
