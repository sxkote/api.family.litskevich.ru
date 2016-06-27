using SXCore.Common.Interfaces;
using SXCore.Common.Services;
using SXCore.Common.Values;
using SXCore.Domain.Entities;
using System;

namespace Litskevich.Family.Domain.Entities
{
    public class ManagerToken : Entity, ICoded
    {
        public string Code { get; private set; }
        public Manager Manager { get; private set; }
        public DateTimeOffset Date { get; private set; }
        public DateTimeOffset Expire { get; private set; }
        public string Data { get; private set; }

        private ManagerToken()
        {
            //this.Code = Guid.NewGuid().ToString();
        }

        static public ManagerToken Create(string code, Manager manager, DateTimeOffset expire, string data)
        {
            return new ManagerToken()
            {
                Code = String.IsNullOrWhiteSpace(code) ? Guid.NewGuid().ToString() : code,
                Manager = manager,
                Date = CommonService.Now,
                Expire = expire,
                Data = data ?? ""
            };
        }
    }
}
