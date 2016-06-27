using SXCore.Common.Enums;
using SXCore.Common.Values;
using System;

namespace Litskevich.Family.WebApi.Models.Requests
{
    public class UpdatePersonRequest
    {
        public PersonTotalName Name { get; set; }
        public Gender Gender { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? DateBirth { get; set; }
        public DateTime? DateDeath { get; set; }
    }
}
