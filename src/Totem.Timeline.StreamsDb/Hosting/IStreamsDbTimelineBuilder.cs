using System;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Timeline.StreamsDb.Hosting
{
  /// <summary>
  /// Describes the configuration of EventStore timeline services
  /// </summary>
  public interface IStreamsDbTimelineBuilder
  {
    IStreamsDbTimelineBuilder ConfigureServices(Action<IServiceCollection> configure);
  }
}