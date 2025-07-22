using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenAppLab.Mod.Posts.Server;
public class Post
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string Type { get; set; }
    public List<PostMeta> Meta { get; set; }
}
public class PostType : ObjectType<Post>
{
    protected override void Configure(IObjectTypeDescriptor<Post> descriptor)
    {
        descriptor
            .Field(p => p.Meta)
            .UseFiltering()
            .UseSorting(); // Optional
    }
}

public class PostMeta
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}

public class PostMetaType : ObjectType<PostMeta>
{
    protected override void Configure(IObjectTypeDescriptor<PostMeta> descriptor)
    {
        // Optional custom configuration
    }
}