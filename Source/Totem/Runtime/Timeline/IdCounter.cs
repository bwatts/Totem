using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// Provides an incremental series of integers
  /// </summary>
  public sealed class IdCounter
  {
    private long _value;

    public IdCounter(long value = 0)
    {
      _value = value;

      Current = Id.From(_value);
      Next = Id.From(_value + 1);
    }

    public Id Current { get; private set; }
    public Id Next { get; private set; }

    public void MoveNext()
    {
      Current = Next;

      _value += 1;

      Next = Id.From(_value + 1);
    }

    public override string ToString() => Current.ToString();
  }
}