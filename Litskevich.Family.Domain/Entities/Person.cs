using SXCore.Common.Entities;
using SXCore.Common.Enums;
using SXCore.Common.Interfaces;
using SXCore.Common.Values;
using System;

namespace Litskevich.Family.Domain.Entities
{
    public class Person : Entity, IEntityWithAvatar
    {
        public Avatar Avatar { get; private set; }
        public Gender Gender { get; private set; }
        public PersonTotalName Name { get; private set; }
        public DateTimeOffset? DateBirth { get; private set; }
        public DateTimeOffset? DateDeath { get; private set; }

        public Manager Manager { get; private set; }

        private Person()
        {
        }

        public override string ToString()
        {
            var familyName = String.IsNullOrEmpty(this.Name.Maiden) ? this.Name.Last : $"{this.Name.Last} ({this.Name.Maiden})";
            return $"{familyName} {this.Name.First} {this.Name.Second}".Trim();
        }

        public void ChangeAvatar(Avatar avatar)
        {
            this.Avatar = avatar;
        }

        static public Person Create(string nameLast, string nameFirst, string nameSecond, string nameMaiden = "", Gender gender = Gender.Unknown, DateTimeOffset? dateBirth = null, DateTimeOffset? dateDeath = null)
        {
            return new Person()
            {
                Gender = gender,
                Name = new PersonTotalName(nameFirst ?? "", nameLast ?? "", nameSecond ?? "", nameMaiden ?? ""),
                DateBirth = dateBirth,
                DateDeath = dateDeath
            };
        }
    }
}
