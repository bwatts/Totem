using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totem.Runtime.Json;
using Totem.Runtime.Timeline;

namespace Totem
{
  /// <summary>
  /// Scenarios involving the <see cref="IdCounter"/> class
  /// </summary>
  class IdCounterSpecs : Specs
  {
    void NextAndCurrent()
    {
      var counter = new IdCounter();
      Expect(counter.Current).Equals(Id.From(0));
      Expect(counter.Next).Equals(Id.From(1));

      counter.MoveNext();
      Expect(counter.Current).Equals(Id.From(1));
      Expect(counter.Next).Equals(Id.From(2));

      counter.MoveNext();
      Expect(counter.Current).Equals(Id.From(2));
      Expect(counter.Next).Equals(Id.From(3));
    }

    void CanBeSerialized()
    {
      var counter = new IdCounter();
      counter.MoveNext();
      counter.MoveNext();

      var json = JsonFormat.Text.Serialize(counter);
      var deserialized = JsonFormat.Text.Deserialize<IdCounter>(json);

      Expect(counter.Current).Equals(deserialized.Current);
      Expect(counter.Next).Equals(deserialized.Next);
    }
  }
}
