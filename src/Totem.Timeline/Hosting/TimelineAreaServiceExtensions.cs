using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Totem.Runtime.Hosting;
using Totem.Timeline.Area;

namespace Totem.Timeline.Hosting
{
  /// <summary>
  /// Extends <see cref="IServiceCollection"/> to declare the timeline area
  /// </summary>
  public static class TimelineAreaServiceExtensions
  {
    public static IServiceCollection AddAreaMap(this IServiceCollection services) =>
      services.AddSingleton(p =>
      {
        var options = p.GetOptions<TimelineAreaOptions>();

        return AreaMap.From(options.Types);
      });

    public static IServiceCollection ConfigureArea(this IServiceCollection services, IEnumerable<Type> types) =>
      services.Configure<TimelineAreaOptions>(options => options.Types.AddRange(types));

    public static IServiceCollection ConfigureArea<TArea>(this IServiceCollection services) where TArea : TimelineArea, new() =>
      services.ConfigureArea(new TArea().GetTypes());
  }
}