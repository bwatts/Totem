using System;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Timeline.EventStore.Hosting
{
  /// <summary>
  /// Describes the configuration of EventStore timeline services
  /// </summary>
  public interface IEventStoreTimelineBuilder
  {
    IEventStoreTimelineBuilder ConfigureServices(Action<IServiceCollection> configure);
  }
}