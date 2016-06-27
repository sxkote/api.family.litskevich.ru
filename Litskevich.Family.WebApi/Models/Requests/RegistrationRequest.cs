using SXCore.Common.Values;

namespace Litskevich.Family.WebApi.Models.Requests
{
    public  class RegistrationRequest
    {
        public PersonTotalName Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
    }
}
