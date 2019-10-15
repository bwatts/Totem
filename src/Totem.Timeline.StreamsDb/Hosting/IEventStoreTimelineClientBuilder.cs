using System;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Timeline.EventStore.Hosting
{
  /// <summary>
  /// Describes the configuration of EventStore timeline client services
  /// </summary>
  public interface IEventStoreTimelineClientBuilder
  {
    IEventStoreTimelineClientBuilder ConfigureServices(Action<IServiceCollection> configure);
  }
}