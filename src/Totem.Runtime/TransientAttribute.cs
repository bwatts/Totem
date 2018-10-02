using System;

namespace Totem.Runtime
{
  /// <summary>
  /// Indicates the decorated field or property is not written to durable representations
  /// and ignored when reading them
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class TransientAttribute : Attribute
  {

  }
}