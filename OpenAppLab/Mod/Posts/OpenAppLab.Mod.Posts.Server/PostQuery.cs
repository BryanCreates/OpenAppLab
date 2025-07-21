using HotChocolate.Data;
using HotChocolate.Types;

namespace OpenAppLab.Mod.Posts.Server;
[ExtendObjectType("Query")]
public class PostQuery
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    public IQueryable<Post> GetPosts()
        => new List<Post>
        {
            new Post
            {
                Id = Guid.NewGuid(),
                Title = "Bingo",
                Content = "For old people!",
                Type = "game",
                Meta = new()
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Type = "MaxPlayers",
                        Value = "10"
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Type = "MinPlayers",
                        Value = "2"
                    }
                }
            },
            new Post
            {
                Id = Guid.NewGuid(),
                Title = "Jenga",
                Content = "Don't let it drop.",
                Type = "game",
                Meta = new()
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Type = "MaxPlayers",
                        Value = "4"
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Type = "MinPlayers",
                        Value = "2"
                    }
                }
            }
            ,
            new Post
            {
                Id = Guid.NewGuid(),
                Title = "No Cheating",
                Content = "don't cheat.",
                Type = "game",
                Meta = new()
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Type = "MaxPlayers",
                        Value = "4"
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Type = "rule",
                        Value = "No Cheating"
                    }
                }
            }
        }.AsQueryable();
}
