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
    readonly AreaMap _area;

    public TimelineJsonFormatOptionsSetup(AreaMap area)
    {
      _area = area;
    }

    public void Configure(JsonFormatOptions options)
    {
      options.SerializerSettings.Converters.AddRange(
        new FlowKeyConverter(_area),
        new TimelinePositionConverter());

      options.DurableTypes.Write.AddRange(
        from type in _area.Types
        select new DurableAreaType(type));
    }
  }
}