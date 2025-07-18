using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAppLab.Core.UI.Shared.GraphQL.Models;
public class GraphQLError
{
    public string Message { get; set; }
    public List<GraphQLLocation> Locations { get; set; }
    public List<JsonElement> Path { get; set; }
    public GraphQLErrorExtensions Extensions { get; set; }
}