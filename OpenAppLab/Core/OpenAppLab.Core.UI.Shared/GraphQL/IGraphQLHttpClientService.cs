using OpenAppLab.Core.UI.Shared.GraphQL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAppLab.Core.UI.Shared.GraphQL;
public interface IGraphQLHttpClientService
{
    //public Task<HttpClient> GetClientAsync();

    public Task<List<T>> ExecuteQueryAsync<T>(string gqlQuery);

    //public Task<GraphQLResponse<TResponse>> QueryAsync<TResponse>(string query);

    //public Task<GraphQLPaginatedResponse<TResponse>?> QueryPaginatedAsync<TResponse>(
    //        string endpoint,
    //        int first,
    //        string? after,
    //        string? before,
    //        int? last,
    //        bool usePagination,
    //        string properties,
    //        List<GraphQLFilter> filters
    //    );

    //public Task<GraphQLResponse<TResponse>> MutateAsync<TResponse, TInput>(
    //    string mutationName,
    //    TInput input);
}
