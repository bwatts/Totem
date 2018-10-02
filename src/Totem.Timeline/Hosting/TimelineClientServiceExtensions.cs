using System;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Totem.Runtime.Hosting;
using Totem.Timeline.Client;

namespace Totem.Timeline.Hosting
{
  /// <summary>
  /// Extends <see cref="IServiceCollection"/> to declare the timeline command host
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TimelineClientServiceExtensions
  {
    public static IServiceCollection AddTimelineClient<TArea>(this IServiceCollection services)
      where TArea : TimelineArea, new()
    {
      services.AddSingleton(_ => new TArea().BuildMap());

      services.AddOptionsSetup<TimelineJsonFormatOptionsSetup>();

      services.AddHostedServiceWith<ICommandHost, CommandHost>();

      return services;
    }

    public static IServiceCollection AddTimelineClient<TArea>(this IServiceCollection services, Action<ITimelineClientBuilder> configure)
      where TArea : TimelineArea, new()
    {
      services.AddTimelineClient<TArea>();

      configure(new TimelineClientBuilder(services));

      return services;
    }

    class TimelineClientBuilder : ITimelineClientBuilder
    {
      readonly IServiceCollection _services;

      internal TimelineClientBuilder(IServiceCollection services)
      {
        _services = services;
      }

      public ITimelineClientBuilder ConfigureServices(Action<IServiceCollection> configure)
      {
        configure(_services);

        return this;
      }
    }
  }
}