using System;
using Newtonsoft.Json;

namespace Totem.Timeline.Json
{
  /// <summary>
  /// Converts <see cref="TimelinePosition"/> to and from JSON as 64-bit integers
  /// </summary>
  /// <remarks>
  /// Javascript only supports literals up to 2^53 - 1 (9007199254740991), technically
  /// making this a lossy conversion. However, we will have more immediate issues if we
  /// ever see timeline positions at that scale.
  /// </remarks>
  public class TimelinePositionConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType) =>
      objectType == typeof(TimelinePosition);

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
      reader.Value == null
        ? TimelinePosition.None
        : new TimelinePosition(Convert.ToInt64(reader.Value));

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      var position = (TimelinePosition) value;

      if(position.IsNone)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteValue(position.ToInt64());
      }
    }
  }
}