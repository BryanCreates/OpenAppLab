using OpenAppLab.Core.UI.Shared;
using OpenAppLab.Core.UI.Shared.GraphQL;
using OpenAppLab.Template.UI.Shared.Services;
using OpenAppLab.Template.UI.Web.Components;
using OpenAppLab.Template.UI.Web.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add device-specific services used by the OpenAppLab.Template.UI.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

//builder.Services.AddHttpClient<GraphQLHttpClientService>(client =>
//{
//    client.BaseAddress = new Uri("https://localhost:7105/graphql");
//});

OpenAppLab.Mod.Posts.UI.Shared.PostsModule.Register();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

var assemblies = new[]
{
    typeof(OpenAppLab.Template.UI.Shared._Imports).Assembly,
    typeof(OpenAppLab.Template.UI.Web.Client._Imports).Assembly
};

var additionalAssemblies = ModuleRegistry.GetAssemblies();

var allAssemblies = assemblies.Concat(additionalAssemblies).ToArray();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(allAssemblies);

app.Run();
