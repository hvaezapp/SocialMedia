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


    public async Task<IEnumerable<string>> SuggestedUsers(long currentUserId, CancellationToken cancellationToken)
    {
       return  await _graphService.SuggestedUsers(currentUserId);
    }
}
