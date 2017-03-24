using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totem.Runtime.Json
{
  /// <summary>
  /// Scenarios involving the <see cref="TotemSerializerSettings"/> class
  /// </summary>
  public class TotemSerializerSettingsSpecs : Specs
  {
    void PreserveUtcDateTime()
    {
      var original = new TimeObject { Time = DateTime.UtcNow };

      var serialized = JsonFormat.Text.Serialize(original);

      var deserialized = JsonFormat.Text.Deserialize<TimeObject>(serialized);

      Expect(original.Time.Hour).Is(deserialized.Time.Hour, "Hour should match original");
      Expect(original.Time.Minute).Is(deserialized.Time.Minute, "Minute should match original");
      Expect(original.Time.Kind).Is(deserialized.Time.Kind, "Kind should match original");
    }

    class TimeObject
    {
      public DateTime Time { get; set; }
    }
  }
}
