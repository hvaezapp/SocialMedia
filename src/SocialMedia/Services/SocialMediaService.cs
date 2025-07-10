using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Entities;
using SocialMedia.Infrastructure.Persistence.Context;

namespace SocialMedia.Services;

public class SocialMediaService(SocialMediaDbContext dbContext , GraphService graphService)
{
    private readonly SocialMediaDbContext _dbContext = dbContext;
    private readonly GraphService _graphService = graphService;    

    public async Task Follow(long followerId , long followingId, CancellationToken cancellationToken)
    {
        // add to sql database

        var newFollow = Domain.Entities.Follow.Create(followerId, followingId);
        _dbContext.Follows.Add(newFollow);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // add to neo4j database

        await _graphService.Follow(newFollow.FollowerId, newFollow.FollowingId);
    }


    public async Task Unfollow(long followerId, long followingId, CancellationToken cancellationToken)
    {
        // delete from sql database

        var follow = await _dbContext.Follows.FirstOrDefaultAsync(a=>a.FollowerId == followerId && a.FollowingId == followingId , cancellationToken);
        if(follow == null)
            throw new ArgumentNullException(nameof(follow) , "The follow relationship between these users does not exist.");

        _dbContext.Follows.Remove(follow);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // remove from neo4j database

        await _graphService.Unfollow(follow.FollowerId, follow.FollowingId);
    }


    public async Task<IEnumerable<string>> SuggestedUsers(long currentUserId, CancellationToken cancellationToken)
    {
       return  await _graphService.SuggestedUsers(currentUserId);
    }
}
