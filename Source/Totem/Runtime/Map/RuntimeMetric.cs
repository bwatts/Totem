using System.Reflection;
using Totem.Metrics;

namespace Totem.Runtime.Map
{
  /// <summary>
  /// A field in a <see cref="RuntimeMetricType"/> monitoring an aspect of runtime performance
  /// </summary>
  public class RuntimeMetric
  {
    public RuntimeMetric(
      RuntimeTypeKey key,
      RuntimeMetricType declaringType,
      FieldInfo declaringField,
      Metric declaration)
    {
      Key = key;
      DeclaringType = declaringType;
      DeclaringField = declaringField;
      Declaration = declaration;
    }

    public readonly RuntimeTypeKey Key;
    public readonly RuntimeMetricType DeclaringType;
    public readonly FieldInfo DeclaringField;
    public readonly Metric Declaration;

    public override string ToString() => Key.ToString();
  }
}