using System.Net.Http.Json;
using System.Text.Json;

namespace OpenAppLab.Core.UI.Shared.GraphQL;
public class GraphQLHttpClientService //: IGraphQLHttpClientService
{
    //public static async Task<List<T>> ExecuteQueryAsync<T>(string gqlQuery)
    //{
    //    var client = new HttpClient();
    //    var response = await client.PostAsJsonAsync("/graphql", new { query = gqlQuery });

    //    var json = await response.Content.ReadAsStringAsync();
    //    var parsed = JsonDocument.Parse(json);
    //    var data = parsed.RootElement.GetProperty("data").GetProperty("posts").GetProperty("items");

    //    return JsonSerializer.Deserialize<List<T>>(data);
    //}

    private readonly HttpClient _httpClient;
    
    public GraphQLHttpClientService(HttpClient httpClient)
    {
        var handler = new HttpClientHandler();

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = httpClient.BaseAddress
        };
    }

    public async Task<List<T>> ExecuteQueryAsync<T>(string gqlQuery)
    {
        var client = new HttpClient();
        var response = await _httpClient.PostAsJsonAsync("/graphql", new { query = gqlQuery });

        var json = await response.Content.ReadAsStringAsync();
        var parsed = JsonDocument.Parse(json);
        var data = parsed.RootElement.GetProperty("data").GetProperty("posts");

        return JsonSerializer.Deserialize<List<T>>(data);
    }
}