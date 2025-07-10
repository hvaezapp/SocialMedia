
namespace SocialMedia.Domain.Entities
{
    public class User
    {
        public const string TableName = "Users";

        public long Id { get; private set; }

        public string Fullname { get; private set; } = null!;
        public string Username { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;

        public ICollection<Follow> Followers { get; private set; } = [];
        public ICollection<Follow> Following { get; private set; } = [];


        private User(string fullname, string username, string passwordHash)
        {
            Fullname = fullname;
            Username = username;
            PasswordHash = passwordHash;
        }


        internal static User Create(string fullname, string username, string passwordHash)
        {
            return new User(fullname , username , passwordHash);
        }
    }
}
