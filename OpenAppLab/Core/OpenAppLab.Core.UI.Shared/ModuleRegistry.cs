using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace OpenAppLab.Core.UI.Shared;
public static class ModuleRegistry
{
    private static readonly List<Assembly> _moduleAssemblies = new();

    public static void AutoRegister(IServiceCollection services)
    {
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location));

        foreach (var assembly in loadedAssemblies)
        {
            if (RegisterAssembly(services, assembly))
            {
                _moduleAssemblies.Add(assembly);
            }
        }
    }

    // Returns true if at least one IModule was registered
    public static bool RegisterAssembly(IServiceCollection services, Assembly assembly)
    {
        var moduleTypes = assembly
            .GetTypes()
            .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

        var anyRegistered = false;

        foreach (var type in moduleTypes)
        {
            var module = (IModule?)Activator.CreateInstance(type);
            if (module != null)
            {
                module.Register(services);
                anyRegistered = true;
            }
        }

        return anyRegistered;
    }

    public static Assembly[] GetAssemblies()
    {
        return _moduleAssemblies.ToArray();
    }
}
