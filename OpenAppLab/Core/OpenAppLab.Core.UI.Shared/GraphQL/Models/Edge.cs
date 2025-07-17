using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAppLab.Core.UI.Shared.GraphQL.Models;
public class Edge<TNode>
{
    public TNode Node { get; set; }
    public string Cursor { get; set; }
}