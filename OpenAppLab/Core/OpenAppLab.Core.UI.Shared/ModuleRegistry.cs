using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenAppLab.Core.UI.Shared;
public static class ModuleRegistry
{
    private static readonly HashSet<Assembly> _assemblies = new();

    public static void RegisterAssembly(Assembly assembly)
    {
        _assemblies.Add(assembly);
    }

    public static Assembly[] GetAssemblies()
    {
        return _assemblies.ToArray();
    }
}