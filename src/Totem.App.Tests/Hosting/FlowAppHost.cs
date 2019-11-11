using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Totem.Timeline;

namespace Totem.App.Tests.Hosting
{
  /// <summary>
  /// Hosts a timeline application for the duration of a flow test
  /// </summary>
  public abstract class FlowAppHost : AppHost
  {
    internal FlowAppHost(Type flowType)
    {
      FlowType = flowType;
    }

    protected readonly Type FlowType;

    protected override IHostBuilder CreateBuilder()
    {
      var builder = new HostBuilder();

      builder.ConfigureServices(ConfigureServices);

      return builder;
    }

    protected abstract void ConfigureServices(IServiceCollection services);

    protected IEnumerable<Type> GetAreaTypes() =>
      from type in FlowType.Assembly.GetTypes()
      where type == FlowType || !typeof(Flow).IsAssignableFrom(type)
      select type;
  }
}