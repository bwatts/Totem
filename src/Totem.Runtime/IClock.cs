using System;

namespace Totem.Runtime
{
  /// <summary>
  /// Describes a context in which events occur on the same timeline
  /// </summary>
  public interface IClock
  {
    DateTimeOffset Now { get; }
  }
}