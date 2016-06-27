namespace Litskevich.Family.WebApi.Models.Requests
{
    public class CreateManagerRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Roles { get; set; }
    }
}
