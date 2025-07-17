using AppAny.HotChocolate.FluentValidation;
using HotChocolate.Execution.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace OpenAppLab.Core.Server.GraphQL;
public static class ServiceCollectionExtensions
{
    public static IRequestExecutorBuilder AddOpenAppLabGraphQL(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder>? dbContextOptions = null)
    {
        // Register ApplicationDbContextFactory
        //services.AddPooledDbContextFactory<ApplicationDbContext>(dbContextOptions ?? (_ => { }));

        // Register GraphQL server with core features
        return services
            .AddGraphQLServer()
            //.RegisterDbContextFactory<ApplicationDbContext>(DbContextKind.Pooled)
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            //.AddAuthorization()
            .AddFluentValidation()
            .AddProjections()
            .AddFiltering()
            .AddSorting();
    }
}
