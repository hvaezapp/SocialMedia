using Neo4j.Driver;

namespace SocialMedia.Services;

public class GraphService(IDriver driver)
{
    private readonly IDriver _driver = driver;

    public async Task CreateUser(long userId)
    {
        var query = @"
                MERGE(u: User { id: $userId })
             ";

        await using var session = _driver.AsyncSession();
        await session.RunAsync(query, new
        {
            userId
        });
    }



    public async Task Follow(long followerId, long followingId)
    {

        var query = @"
                MATCH (a:User { id: $followerId })
                MATCH (b:User { id: $followingId })
                MERGE (a)-[:FOLLOWS]->(b)
            ";

        await using var session = _driver.AsyncSession();
        await session.RunAsync(query, new
        {
            followerId,
            followingId
        });
    }


    public async Task Unfollow(long followerId, long followingId)
    {
        var query = @"
            MATCH (a:User { id: $followerId })-[r:FOLLOWS]->(b:User { id: $followingId })
            DELETE r
        ";

        await using var session = _driver.AsyncSession();
        await session.RunAsync(query, new
        {
            followerId,
            followingId
        });
    }




    public async Task<IEnumerable<string>> SuggestedUsers(long userId)
    {
        var query = @"
                    MATCH (me:User { id: $userId })-[:FOLLOWS]->(:User)-[:FOLLOWS]->(suggested:User)
                    WHERE NOT (me)-[:FOLLOWS]->(suggested) AND me <> suggested
                    RETURN DISTINCT suggested.username AS username
                    LIMIT 20
                ";

        var suggestedUserIds = new List<string>();

        await using var session = _driver.AsyncSession();
        var result = await session.RunAsync(query, new
        {
            userId
        });

        var records = await result.ToListAsync();

        foreach (var record in records)
        {
            suggestedUserIds.Add(record["id"].As<string>());
        }

        return suggestedUserIds;

    }

}
