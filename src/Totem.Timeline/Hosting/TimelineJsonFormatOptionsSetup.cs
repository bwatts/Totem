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
      AddConverters(options);
      AddDurableTypes(options);
    }

    void AddConverters(JsonFormatOptions options) =>
      options.SerializerSettings.Converters.AddRange(
        new FlowKeyConverter(_area),
        new TimelinePositionConverter());

    void AddDurableTypes(JsonFormatOptions options)
    {
      var potentialTypes =
        _area.Types
        .Select(type => type.DeclaredType.Assembly)
        .Distinct()
        .SelectMany(assembly => assembly.GetTypes());

      foreach(var type in potentialTypes)
      {
        options.TryAddDurableType(type);
      }
    }
  }
}