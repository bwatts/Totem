using System;

namespace Totem.Runtime
{
  /// <summary>
  /// Indicates instances of the decorated type persist between usages
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public sealed class DurableAttribute : Attribute
  {

  }
}