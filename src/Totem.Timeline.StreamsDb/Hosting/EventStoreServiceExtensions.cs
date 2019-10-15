using System;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Totem.Runtime.Hosting;
using Totem.Runtime.Json;
using Totem.Timeline.Area;
using Totem.Timeline.Hosting;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.EventStore.Hosting
{
  /// <summary>
  /// Extends <see cref="ITimelineBuilder"/> to declare the EventStore timeline database
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class EventStoreServiceExtensions
  {
    public static IEventStoreTimelineBuilder AddEventStore(this ITimelineBuilder timeline)
    {
      timeline.ConfigureServices(services =>
      {
        services.AddSingleton<ITimelineDb>(p => new TimelineDb(
          p.GetRequiredService<EventStoreContext>(),
          p.GetRequiredService<IResumeProjection>()));

        services.AddSingleton(p => new EventStoreContext(
          p.GetRequiredService<IJsonFormat>(),
          p.GetRequiredService<AreaMap>()));

        //services.AddSingleton<IResumeProjection>(p => new ResumeProjection(
        //  p.GetRequiredService<AreaMap>(),
        //  p.GetRequiredService<ProjectionsManager>(),
        //  p.BuildProjectionsCredentials()));

        //services.AddSingleton(BuildProjectionsManager);
      });

      return new EventStoreTimelineBuilder(timeline);
    }

    public static IEventStoreTimelineBuilder BindOptionsToConfiguration(this IEventStoreTimelineBuilder timeline, string key = "totem.timeline.eventStore") =>
      timeline.ConfigureServices(services => services.BindOptionsToConfiguration<EventStoreTimelineOptions>(key));

    class EventStoreTimelineBuilder : IEventStoreTimelineBuilder
    {
      readonly ITimelineBuilder _timeline;

      internal EventStoreTimelineBuilder(ITimelineBuilder timeline)
      {
        _timeline = timeline;
      }

      public IEventStoreTimelineBuilder ConfigureServices(Action<IServiceCollection> configure)
      {
        _timeline.ConfigureServices(configure);

        return this;
      }
    }
  }
}