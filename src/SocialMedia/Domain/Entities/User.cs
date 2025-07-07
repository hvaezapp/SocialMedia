namespace SocialMedia.Domain.Entities
{
    public class User
    {
        public const string TableName = "Users";

        public long Id { get; private set; }  

        public string Username { get; private set; } = null!;
        public string Fullname { get; private set; } = null!;

        public ICollection<Follow> Followers { get; private set; } = [];
        public ICollection<Follow> Following { get; private set; } = [];


    }
}
