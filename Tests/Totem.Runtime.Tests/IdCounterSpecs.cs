using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Json;
using Totem.Runtime.Timeline;

namespace Totem.Runtime
{
  /// <summary>
  /// Scenarios involving the <see cref="IdCounter"/> class
  /// </summary>
  public class IdCounterSpecs : Specs
  {
		void NoMoves()
		{
			var counter = new IdCounter();

			Expect(counter.Current).Is(Id.From(0));
			Expect(counter.Next).Is(Id.From(1));
		}

		void MoveOnce()
		{
			var counter = new IdCounter();

			counter.MoveNext();

			Expect(counter.Current).Is(Id.From(1));
			Expect(counter.Next).Is(Id.From(2));
		}

		void MoveTwice()
    {
      var counter = new IdCounter();

      counter.MoveNext();
      counter.MoveNext();

      Expect(counter.Current).Is(Id.From(2));
      Expect(counter.Next).Is(Id.From(3));
    }

    void Serialize()
    {
      var counter = new IdCounter();

      counter.MoveNext();
      counter.MoveNext();

      var json = JsonFormat.Text.Serialize(counter);
      var deserialized = JsonFormat.Text.Deserialize<IdCounter>(json);

      Expect(counter.Current).Is(deserialized.Current);
      Expect(counter.Next).Is(deserialized.Next);
    }
  }
}