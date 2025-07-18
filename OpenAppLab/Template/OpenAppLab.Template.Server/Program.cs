using OpenAppLab.Core.Server.GraphQL;
using OpenAppLab.Mod.Posts.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var graphQL = builder.Services.AddOpenAppLabGraphQL(options =>
{
    //options.UseSqlite("Data Source=app.db"); // or UseMySql, etc.
});

//Register Modules
graphQL.AddPostsModule();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.MapGraphQL();

app.Run();