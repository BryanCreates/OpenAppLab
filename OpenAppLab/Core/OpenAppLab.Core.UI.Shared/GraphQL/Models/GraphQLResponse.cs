﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAppLab.Core.UI.Shared.GraphQL.Models;
public class GraphQLResponse<T>
{
    public T Data { get; set; }
    public List<GraphQLError> Errors { get; set; }
}
