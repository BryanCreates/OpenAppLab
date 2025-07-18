using Microsoft.Extensions.DependencyInjection;
using OpenAppLab.Core.UI.Shared;
using System.Reflection;

namespace OpenAppLab.Template.UI.Shared;
public static class ModuleRegistry
{
    public static Assembly[] KnownAssemblies = new[]
    {
        typeof(OpenAppLab.Mod.Posts.UI.Shared.PostsModule).Assembly,
    };

    public static void AutoRegister(IServiceCollection services)
    {
        foreach (var assembly in KnownAssemblies)
        {
            RegisterAssembly(services, assembly);
        }
    }

    public static void RegisterAssembly(IServiceCollection services, Assembly assembly)
    {
        var moduleTypes = assembly
            .GetTypes()
            .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

        foreach (var type in moduleTypes)
        {
            var module = (IModule)Activator.CreateInstance(type)!;
            module.Register(services);
        }
    }
}

