namespace Litskevich.Family.WebApi.Models
{
    public class AuthorModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }

        public AuthorModel()
        {
        }

        public AuthorModel(long id, string name, string avatar = "")
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
