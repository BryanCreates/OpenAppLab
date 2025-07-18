using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAppLab.Core.UI.Shared.GraphQL.Models;
public class GraphQLLocation
{
    public int Line { get; set; }
    public int Column { get; set; }
}