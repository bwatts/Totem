using System.Linq;
using Microsoft.Extensions.Options;
using Totem.Runtime.Hosting;
using Totem.Timeline.Area;
using Totem.Timeline.Json;

namespace Totem.Timeline.Hosting
{
  /// <summary>
  /// Configures the Totem JSON format with converters and the area's durable types
  /// </summary>
  public class TimelineJsonFormatOptionsSetup : IConfigureOptions<JsonFormatOptions>
  {
    readonly AreaMap _map;

    public TimelineJsonFormatOptionsSetup(AreaMap map)
    {
      _map = map;
    }

    public void Configure(JsonFormatOptions options)
    {
      options.SerializerSettings.Converters.AddRange(
        new FlowKeyConverter(_map),
        new TimelinePositionConverter());

      options.DurableTypes.Write.AddRange(
        from type in _map.Types
        select new DurableMapType(type));
    }
  }
}