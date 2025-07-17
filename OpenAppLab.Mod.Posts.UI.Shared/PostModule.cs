using OpenAppLab.Core.UI.Shared;

namespace OpenAppLab.Mod.Posts.UI.Shared;
public static class PostsModule
{
    static PostsModule()
    {
        ModuleRegistry.RegisterAssembly(typeof(Component1).Assembly);
    }

    // Optional: Call this from app startup instead of relying on static constructor
    public static void Register() { /* Ensures static ctor is triggered */ }
}