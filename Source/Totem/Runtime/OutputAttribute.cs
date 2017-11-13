using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totem.Runtime
{
  /// <summary>
  /// Indicates the decorated type will be serialized as JSON, but never deserialized.
  /// Useful if the property always needs to be set through code.
  /// </summary>
  public sealed class OutputAttribute : Attribute
  {

  }
}
