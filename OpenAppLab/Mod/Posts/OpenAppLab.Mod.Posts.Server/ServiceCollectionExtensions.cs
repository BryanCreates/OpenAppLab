using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OpenAppLab.Mod.Posts.Server;
public static class ServiceCollectionExtensions
{
    public static IRequestExecutorBuilder AddPostsModule(this IRequestExecutorBuilder builder)
    {
        builder.AddTypeExtension<PostQuery>();
        builder.AddType<PostType>();
        builder.AddType<PostMetaType>();

        builder.AddTypeExtension<PostMutation>();

        return builder;
    }
}