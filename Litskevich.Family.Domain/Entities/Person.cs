using SXCore.Common.Enums;
using SXCore.Common.Values;
using SXCore.Domain.Entities;
using SXCore.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace Litskevich.Family.Domain.Entities
{
    public class Person : Entity, IEntityWithAvatar
    {
        public Avatar Avatar { get; private set; }
        public Gender Gender { get; private set; }
        public PersonTotalName Name { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public DateTime? DateBirth { get; private set; }
        public DateTime? DateDeath { get; private set; }

        public Manager Manager { get; private set; }

        private Person()
        {
        }

        public override string ToString()
        {
            var familyName = String.IsNullOrEmpty(this.Name.Maiden) ? this.Name.Last : $"{this.Name.Last} ({this.Name.Maiden})";
            return $"{familyName} {this.Name.First} {this.Name.Second}".Trim();
        }

        public void Change(Gender gender, PersonTotalName name, string email = "", string phone = "", DateTime? dateBirth = null, DateTime? dateDeath = null)
        {
            this.Gender = gender;
            this.Name = name;
            this.Email = email;
            this.Phone = phone;
            this.DateBirth = dateBirth?.Date;
            this.DateDeath = dateDeath?.Date;
        }

        public void ChangeAvatar(Avatar avatar)
        {
            this.Avatar = avatar;
        }

        public Manager CreateManager(string login, string password = "", ICollection<ManagerRoleType> roles = null)
        {
            return this.Manager = Manager.Create(this, login, password, roles);
        }

        static public Person Create(Gender gender, PersonTotalName name, string email = "", string phone = "", DateTime? dateBirth = null, DateTime? dateDeath = null)
        {
            return new Person()
            {
                Gender = gender, 
                Name = name,
                Email = email ?? "",
                Phone = phone ?? "",
                DateBirth = dateBirth?.Date,
                DateDeath = dateDeath?.Date
            };
        }
    }
}
