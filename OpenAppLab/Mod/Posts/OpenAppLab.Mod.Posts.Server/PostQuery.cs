using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAppLab.Mod.Posts.Server;
[ExtendObjectType("Query")]
public class PostQuery
{
    public IQueryable<Post> GetPosts()
        => new List<Post>
        {
            new Post
            {
                Id = Guid.NewGuid(),
                Title = "Hello World",
                Content = "This is my first post!"
            },
            new Post
            {
                Id = Guid.NewGuid(),
                Title = "GraphQL is Awesome",
                Content = "Let's learn more about GraphQL."
            }
        }.AsQueryable();

    //public IQueryable<Post> GetPosts([Service] ApplicationDbContext db)
    //    => db.Set<Post>().AsQueryable();
}
