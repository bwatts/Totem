using System;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Totem.Runtime.Hosting;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.Hosting
{
  /// <summary>
  /// Extends <see cref="IServiceCollection"/> to declare the timeline host
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TimelineServiceExtensions
  {
    public static IServiceCollection AddTimeline<TArea>(this IServiceCollection services)
      where TArea : TimelineArea, new()
    {
      services.AddSingleton(_ => new TArea().BuildMap());

      services.AddOptionsSetup<TimelineJsonFormatOptionsSetup>();

      services.AddHostedService<TimelineHost>();

      return services;
    }

    public static IServiceCollection AddTimeline<TArea>(this IServiceCollection services, Action<ITimelineBuilder> configure)
      where TArea : TimelineArea, new()
    {
      services.AddTimeline<TArea>();

      configure(new TimelineBuilder(services));

      return services;
    }

    class TimelineBuilder : ITimelineBuilder
    {
      readonly IServiceCollection _services;

      internal TimelineBuilder(IServiceCollection services)
      {
        _services = services;
      }

      public ITimelineBuilder ConfigureServices(Action<IServiceCollection> configure)
      {
        configure(_services);

        return this;
      }
    }
  }
}