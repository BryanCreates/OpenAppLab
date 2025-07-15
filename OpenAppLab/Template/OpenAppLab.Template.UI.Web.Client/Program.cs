using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenAppLab.Template.UI.Shared.Services;
using OpenAppLab.Template.UI.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the OpenAppLab.Template.UI.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

await builder.Build().RunAsync();
