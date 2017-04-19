using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Totem.Runtime;

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

    [Transient, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Fields IBindable.Fields => Fields;

    [Transient, DebuggerHidden, DebuggerNonUserCode, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected internal Fields Fields { get; internal set; }
  }
}