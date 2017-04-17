using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem
{
  /// <summary>
  /// An object hosting a set of bindable, observable fields
  /// </summary>
  public abstract class Binding : Clean, IBindable
  {
    protected Binding()
    {
      Fields = new Fields(this);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Fields IBindable.Fields => Fields;

    [DebuggerHidden, DebuggerNonUserCode, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected Fields Fields { get; }
  }
}