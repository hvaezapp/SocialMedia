using Neo4j.Driver;
using SocialMedia.Domain.Entities;

namespace SocialMedia.Services
{
    public class SocialMediaService(IDriver driver)
    {
        private readonly IDriver _driver = driver;

        public async Task CreateUser(string Username)
        {
            var query = @"
                MERGE(u: User { username: $username })
             ";

            await using var session = _driver.AsyncSession();
            await session.RunAsync(query, new
            {
                username = Username,
            });
        }



        public async Task Follow(string currentUsername , string targetUsername)
        {
            
            var query = @"
                MATCH (a:User { username: $currentUsername })
                MATCH (b:User { username: $targetUsername })
                MERGE (a)-[:FOLLOWS]->(b)
            ";


            await using var session = _driver.AsyncSession();
            await session.RunAsync(query, new
            {
                currentUsername,
                targetUsername
            });
        }


        public async Task<IEnumerable<string>> SuggestedUsers(string Username)
        {
            var query = @"
                    MATCH (me:User { username: $username })-[:FOLLOWS]->(:User)-[:FOLLOWS]->(suggested:User)
                    WHERE NOT (me)-[:FOLLOWS]->(suggested) AND me <> suggested
                    RETURN DISTINCT suggested.username AS username
                    LIMIT 10
                ";

            var suggestedUsernames = new List<string>();

            await using var session = _driver.AsyncSession();
            var result = await session.RunAsync(query, new
            {
                username = Username
            });

            var records = await result.ToListAsync();

            foreach (var record in records)
            {
                suggestedUsernames.Add(record["username"].As<string>());
            }

            return suggestedUsernames;

        }

    }
}
