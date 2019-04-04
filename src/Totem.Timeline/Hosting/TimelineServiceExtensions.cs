using System;
using Microsoft.Extensions.DependencyInjection;
using Totem.Runtime.Hosting;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.Hosting
{
  /// <summary>
  /// Extends <see cref="IServiceCollection"/> to declare the timeline host
  /// </summary>
  public static class TimelineServiceExtensions
  {
    public static IServiceCollection AddTimeline(this IServiceCollection services) =>
      services
      .AddAreaMap()
      .AddOptionsSetup<TimelineJsonFormatOptionsSetup>()
      .AddHostedService<TimelineHost>();

    public static IServiceCollection AddTimeline(this IServiceCollection services, Action<ITimelineBuilder> configure)
    {
      services.AddTimeline();

      configure(new TimelineBuilder(services));

      return services;
    }

    public static IServiceCollection AddTimeline<TArea>(this IServiceCollection services) where TArea : TimelineArea, new() =>
      services.AddTimeline().ConfigureArea<TArea>();

    public static IServiceCollection AddTimeline<TArea>(this IServiceCollection services, Action<ITimelineBuilder> configure) where TArea : TimelineArea, new() =>
      services.AddTimeline(configure).ConfigureArea<TArea>();

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