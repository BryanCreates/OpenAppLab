using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAppLab.Mod.Posts.Server;

[ExtendObjectType("Mutation")]
public class PostMutation
{
    public async Task<Post> CreatePostAsync()
    {
        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = "Hello World",
            Content = "This is my first post!"
        };

        return post;
    }
}
