using System;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Timeline.StreamsDb.Hosting
{
  /// <summary>
  /// Describes the configuration of EventStore timeline client services
  /// </summary>
  public interface IStreamsDbTimelineClientBuilder
  {
    IStreamsDbTimelineClientBuilder ConfigureServices(Action<IServiceCollection> configure);
  }
}