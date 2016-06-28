namespace Litskevich.Family.WebApi.Models.Requests
{
    public class InviteGuestRequest
    {
        public string NameLast { get; set; }
        public string NameFirst { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int Hours { get; set; }

        public InviteGuestRequest()
        {

        }
    }
}
