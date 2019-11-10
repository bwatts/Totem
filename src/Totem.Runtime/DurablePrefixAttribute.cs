using System;
using Totem.Runtime.Json;

namespace Totem.Runtime
{
  /// <summary>
  /// Indicates the prefix of the durable types of the decorated class or assembly
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
  public sealed class DurablePrefixAttribute : Attribute
  {
    public DurablePrefixAttribute(string prefix)
    {
      Prefix = prefix;
    }

    public readonly string Prefix;
  }
}