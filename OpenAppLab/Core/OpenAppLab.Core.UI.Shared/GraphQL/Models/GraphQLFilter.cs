using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAppLab.Core.UI.Shared.GraphQL.Models;
public class GraphQLFilter
{
    public string? Field { get; set; } // null when this is a group (e.g., an "or" block)
    public string Operator { get; set; } = "contains";
    public object? Value { get; set; }
    public string? GroupOperator { get; set; } // "and", "or", "not"
    public List<GraphQLFilter>? Filters { get; set; } // nested filters
}