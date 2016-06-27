using Litskevich.Family.Domain.Values;
using SXCore.Common.Enums;
using SXCore.Common.Values;
using System;

namespace Litskevich.Family.WebApi.Models
{
    public class PersonModel
    {
        public long ID { get; set; }

        public string Avatar { get; set; }

        public Gender Gender { get; set; }
        public PersonTotalName Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? DateBirth { get; set; }
        public DateTime? DateDeath { get; set; }
    }
}
