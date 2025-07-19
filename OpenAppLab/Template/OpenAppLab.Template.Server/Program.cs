using OpenAppLab.Core.Server.GraphQL;
using OpenAppLab.Mod.Posts.Server;

var allowCorsOrigins = "_allowCorsOrigins";

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(allowCorsOrigins, policy =>
    {
        policy.WithOrigins(allowedOrigins ?? Array.Empty<string>())
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// Add services to the container.

var graphQL = builder.Services.AddOpenAppLabGraphQL(options =>
{
    //options.UseSqlite("Data Source=app.db"); // or UseMySql, etc.
});

//Register Modules
graphQL.AddPostsModule();


var app = builder.Build();

app.UseCors("_allowCorsOrigins");

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.MapGraphQL();

app.Run();