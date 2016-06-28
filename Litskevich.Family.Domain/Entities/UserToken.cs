using SXCore.Common.Interfaces;
using SXCore.Common.Services;
using SXCore.Common.Values;
using SXCore.Domain.Entities;
using System;

namespace Litskevich.Family.Domain.Entities
{
    public class UserToken : Entity, ICoded
    {
        public string Code { get; private set; }
        public Manager Manager { get; private set; }
        public Guest Guest { get; private set; }
        public DateTimeOffset Date { get; private set; }
        public DateTimeOffset Expire { get; private set; }
        public string Data { get; private set; }

        private UserToken()
        {
            this.Code = Guid.NewGuid().ToString();
        }

        static public UserToken Create(Manager manager, Guest guest, DateTimeOffset expire, string data = "")
        {
            return new UserToken()
            {
                Manager = manager,
                Guest = guest,
                Date = CommonService.Now,
                Expire = expire,
                Data = data ?? ""
            };
        }
    }
}
