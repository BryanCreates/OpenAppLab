﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAppLab.Core.UI.Shared;
public interface IModule
{
    void Register(IServiceCollection services);
}