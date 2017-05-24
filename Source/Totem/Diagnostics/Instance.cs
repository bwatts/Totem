using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// The name of an instance of a multi-counter. Implicitly converts from string and <see cref="Id"/>.
  /// </summary>
  public struct Instance
  {
    readonly string _name;

    Instance(string name) : this()
    {
      _name = name;
    }

    public override string ToString() => _name;

    public static implicit operator Instance(string name) => new Instance(name);
    public static implicit operator Instance(Id id) => new Instance(id.ToString());
  }
}