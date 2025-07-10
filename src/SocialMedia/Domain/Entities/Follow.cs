namespace SocialMedia.Domain.Entities
{
    public class Follow
    {
        public const string TableName = "Follows";

        public long Id { get; private set; }

        public long FollowerId { get; private set; } 
        public User Follower { get; private set; } = null!;

        public long FollowingId { get; private set; }
        public User Following { get; private set; } = null!;

        public DateTime CreatedOn { get; private set; }


        private Follow(long followerId , long followingId)
        {
            FollowerId = followerId;    
            FollowingId = followingId;
            CreatedOn = DateTime.Now;
        }

        public static Follow Create(long followerId, long followingId)
        {
            return new Follow(followerId, followingId);
        }
    }

}
