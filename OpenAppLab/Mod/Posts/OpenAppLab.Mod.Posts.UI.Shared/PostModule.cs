using Microsoft.Extensions.DependencyInjection;
using OpenAppLab.Core.UI.Shared;

namespace OpenAppLab.Mod.Posts.UI.Shared;
public class PostsModule : IModule
{
    public void Register(IServiceCollection services)
    {
        Console.WriteLine("PostsModule registered!");
        //ModuleRegistry.RegisterAssembly(typeof(PostsModule).Assembly);
    }
}