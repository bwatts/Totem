using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// Provides an incremental series of integers
  /// </summary>
  [Durable]
  public sealed class IdCounter
  {
    private long _value;

    public IdCounter(long value = 0)
    {
      _value = value;
    }

    [Transient]
    public Id Current => Id.From(_value);
    [Transient]
    public Id Next => Id.From(_value + 1);

    public void MoveNext()
    {
      _value += 1;
    }

    public override string ToString() => Current.ToString();
  }
}