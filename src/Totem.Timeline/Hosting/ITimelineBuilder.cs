using System;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Timeline.Hosting
{
  /// <summary>
  /// Describes the configuration of the hosted timeline
  /// </summary>
  public interface ITimelineBuilder
  {
    ITimelineBuilder ConfigureServices(Action<IServiceCollection> configure);
  }
}