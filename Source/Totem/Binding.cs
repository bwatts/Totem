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
  /// <remarks>
  /// Fields is implemented as a lazy initialization to ensure deserialized durable instances
  /// have a valid, instantiated set of fields.
  /// 
  /// Other classes implementing <see cref="IBindable"/> can choose not to use the lazy
  /// instantiation if they are not durable.
  /// </remarks>
  public abstract class Binding : Clean, IBindable
  {
    Fields _fields;

    [Transient, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Fields IBindable.Fields => Fields;

    [Transient, DebuggerHidden, DebuggerNonUserCode, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected internal Fields Fields => _fields ?? (_fields = new Fields());
  }
}