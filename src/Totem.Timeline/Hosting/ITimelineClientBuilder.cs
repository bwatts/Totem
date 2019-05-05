using System;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Timeline.Hosting
{
  /// <summary>
  /// Describes the configuration of the hosted timeline client
  /// </summary>
  public interface ITimelineClientBuilder
  {
    ITimelineClientBuilder ConfigureServices(Action<IServiceCollection> configure);
  }
}