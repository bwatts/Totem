using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime;

namespace Totem
{
  /// <summary>
  /// A signal on the timeline of a distributed environment
  /// </summary>
  [Durable]
  public abstract class Event : Notion
  {
    protected Event()
    {
      When = Clock.Now;
    }

    public DateTime When
    {
      get { return Traits.When.Get(this); }
      private set { Traits.When.Set(this, value); }
    }

    public new static class Traits
    {
      public static readonly Field<DateTime> When = Field.Declare(() => When);
    }
  }
}