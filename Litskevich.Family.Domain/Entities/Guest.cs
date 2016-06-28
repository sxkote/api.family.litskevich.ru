using SXCore.Common.Exceptions;
using SXCore.Common.Services;
using SXCore.Common.Values;
using SXCore.Domain.Entities;
using System;

namespace Litskevich.Family.Domain.Entities
{
    public class Guest : Entity
    {
        public PersonName Name { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public DateTimeOffset Date { get; set; }
        public DateTimeOffset Expire { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }

        public long CreatedByPersonID { get; set; }

        public void Disable()
        {
            this.Expire = CommonService.Now;
        }

        static public Guest Create(Person author, PersonName name, string email, string phone, DateTimeOffset expire, string login, string password)
        {
            if (author == null)
                throw new CustomArgumentException("Invalid author for Guest!");

            return new Guest()
            {
                CreatedByPersonID = author.ID,
                Name = name,
                Email = email ?? "",
                Phone = phone ?? "",
                Date = CommonService.Now,
                Expire = expire,
                Login = login ?? "guest",
                Password = CommonService.HashPassword(password, 10)
            };
        }
    }
}
