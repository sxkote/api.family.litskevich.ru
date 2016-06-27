using SXCore.Common.Enums;
using SXCore.Common.Values;
using System;

namespace Litskevich.Family.Domain.Values
{
    public class PersonData
    {
        public Gender Gender { get; set; }
        public PersonTotalName Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? DateBirth { get; set; }
        public DateTime? DateDeath { get; set; }
    }
}
