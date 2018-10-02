using System;
using Newtonsoft.Json;
using Totem.Timeline.Area;

namespace Totem.Timeline.Json
{
  /// <summary>
  /// Converts <see cref="FlowKey"/> to and from JSON
  /// </summary>
  public class FlowKeyConverter : JsonConverter
  {
    readonly AreaMap _area;

    public FlowKeyConverter(AreaMap area)
    {
      _area = area;
    }

    public override bool CanConvert(Type objectType) =>
      objectType == typeof(FlowKey);

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
      reader.Value == null ? null : FlowKey.From(_area, reader.Value.ToString());

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
      writer.WriteValue(value?.ToString());
  }
}