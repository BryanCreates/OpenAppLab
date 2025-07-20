using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenAppLab.Core.UI.Shared.GraphQL.Models;
public class Edge<TNode>
{
    [JsonPropertyName("node")]
    public TNode Node { get; set; }
    [JsonPropertyName("cursor")]
    public string Cursor { get; set; }
}