using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenAppLab.Core.UI.Shared.GraphQL.Models;
public class GraphQLPaginatedResponse<TNode>
{
    public Edge<TNode>[] Edges { get; set; }
    public PageInfo PageInfo { get; set; }
    public List<GraphQLError> Errors { get; set; }
}
