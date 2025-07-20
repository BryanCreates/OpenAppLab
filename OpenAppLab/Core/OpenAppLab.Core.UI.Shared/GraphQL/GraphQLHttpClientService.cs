using OpenAppLab.Core.UI.Shared.GraphQL.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace OpenAppLab.Core.UI.Shared.GraphQL;
public class GraphQLHttpClientService
{
    private readonly HttpClient _httpClient;
    
    public GraphQLHttpClientService(HttpClient httpClient)
    {
        var handler = new HttpClientHandler();

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = httpClient.BaseAddress
        };
    }

    public async Task<GraphQLPaginatedResponse<TNode>> ExecuteQueryAsync<TNode>(string gqlQuery, string rootFieldName)
    {
        var response = await _httpClient.PostAsJsonAsync("/graphql", new { query = gqlQuery });
        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        var errors = root.TryGetProperty("errors", out var errorsElement)
            ? JsonSerializer.Deserialize<List<GraphQLError>>(errorsElement.GetRawText())
            : null;

        var postsElement = root.GetProperty("data").GetProperty(rootFieldName);
        var parsedResult = JsonSerializer.Deserialize<GraphQLPaginatedResponse<TNode>>(postsElement.GetRawText());

        if (parsedResult != null)
            parsedResult.Errors = errors;

        return parsedResult!;
    }
}