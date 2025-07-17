using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenAppLab.Core.UI.Shared.GraphQL;
using OpenAppLab.Template.UI.Shared.Services;
using OpenAppLab.Template.UI.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the OpenAppLab.Template.UI.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

builder.Services.AddHttpClient<GraphQLHttpClientService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7105/graphql");
});

await builder.Build().RunAsync();
