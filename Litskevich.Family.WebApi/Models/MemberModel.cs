using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litskevich.Family.WebApi.Models
{
    public class MemberModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }

        public MemberModel()
        {
        }

        public MemberModel(long id, string name, string avatar = "")
        {
            this.ID = id;
            this.Name = name;
            this.Avatar = avatar;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
