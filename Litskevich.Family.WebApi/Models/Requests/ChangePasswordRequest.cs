using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litskevich.Family.WebApi.Models.Requests
{
    public class ChangePasswordRequest
    {
        public string PasswordOld { get; set; }
        public string PasswordNew { get; set; }
    }
}
