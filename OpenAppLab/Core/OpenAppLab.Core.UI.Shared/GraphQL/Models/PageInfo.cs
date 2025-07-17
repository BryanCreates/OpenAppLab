using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAppLab.Core.UI.Shared.GraphQL.Models;
public class PageInfo
{
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    public string StartCursor { get; set; }
    public string EndCursor { get; set; }
}