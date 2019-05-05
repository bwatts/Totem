using System;
using Newtonsoft.Json;
using Totem.Timeline.Area;

namespace Totem.Timeline.Json
{
  /// <summary>
  /// Converts instances of <see cref="FlowKey"/> to and from JSON
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
      reader.Value == null ? null : FlowKey.From(reader.Value.ToString(), _area);

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
      writer.WriteValue(value?.ToString());
  }
}