using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
  /// <summary>
  /// Describes an objecth hosting a set of bindable, observable fields
  /// </summary>
  public interface IBindable
  {
    Fields Fields { get; }
  }
}