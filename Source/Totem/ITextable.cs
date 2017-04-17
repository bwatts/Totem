using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
  /// <summary>
  /// Describes an object with a <see cref="Text"/> representation
  /// </summary>
  public interface ITextable : IClean
  {
    Text ToText();
  }
}