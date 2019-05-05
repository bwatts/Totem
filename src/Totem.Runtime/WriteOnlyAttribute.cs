using System;

namespace Totem.Runtime
{
  /// <summary>
  /// Indicates the decorated field or property is written to durable representations
  /// but ignored when reading them
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class WriteOnlyAttribute : Attribute
  {

  }
}