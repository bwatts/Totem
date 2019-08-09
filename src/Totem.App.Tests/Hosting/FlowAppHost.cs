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
    readonly Type _flowType;

    internal FlowAppHost(Type flowType)
    {
      _flowType = flowType;
    }

    protected override IHostBuilder CreateBuilder()
    {
      var builder = new HostBuilder();

      builder.ConfigureServices(ConfigureServices);

      return builder;
    }

    protected abstract void ConfigureServices(IServiceCollection services);

    protected IEnumerable<Type> GetAreaTypes()
    {
      yield return _flowType;

      foreach(var eventType in GetEventTypes())
      {
        yield return eventType;
      }
    }

    IEnumerable<Type> GetEventTypes() =>
      _flowType.Assembly.GetTypes().Where(typeof(Event).IsAssignableFrom);
  }
}