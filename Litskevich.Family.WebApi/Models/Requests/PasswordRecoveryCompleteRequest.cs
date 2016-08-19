using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Litskevich.Family.WebApi.Models.Requests
{
    public class PasswordRecoveryCompleteRequest
    {
        public string Code { get; set; }
    }
}