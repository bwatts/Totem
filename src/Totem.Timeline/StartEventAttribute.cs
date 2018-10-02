using System;

namespace Totem.Timeline
{
  /// <summary>
  /// Indicates the decorated Start method of a request may yield a type of event
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
  public sealed class StartEventAttribute : Attribute
  {
    public StartEventAttribute(Type type)
    {
      Type = type;
    }

    public readonly Type Type;
  }
}