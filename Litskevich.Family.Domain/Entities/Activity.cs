using SXCore.Common.Interfaces;
using SXCore.Common.Services;
using SXCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litskevich.Family.Domain.Entities
{
    public class Activity : Entity, ICoded
    {
        public string Code { get; private set; }
        public string Identifier { get; private set; }
        public DateTimeOffset Date { get; private set; }
        public DateTimeOffset Expire { get; private set; }
        public string Data { get; private set; }

        private Activity() { }

        public bool IsValid()
        {
            var now = CommonService.Now;
            return this.Date <= now && now <= this.Expire;
        }

        public void Discard()
        {
            this.Expire = CommonService.Now;
        }

        static public Activity Create(DateTimeOffset expire, string identifier, string data)
        {
            return new Activity()
            {
                Code = Guid.NewGuid().ToString(),
                Identifier = identifier ?? "",
                Date = CommonService.Now,
                Expire = expire,
                Data = data ?? ""
            };
        }
    }
}
