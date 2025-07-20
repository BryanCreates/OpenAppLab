using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenAppLab.Core.UI.Shared.GraphQL.Models;
public class GraphQLPaginatedResponse<TNode>
{
    [JsonPropertyName("edges")]
    public Edge<TNode>[] Edges { get; set; }
    [JsonPropertyName("pageInfo")]
    public PageInfo PageInfo { get; set; }
    [JsonPropertyName("errors")]
    public List<GraphQLError> Errors { get; set; }
}
