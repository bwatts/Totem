using System.Reflection;
using Totem.Runtime.Metrics;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A field in a <see cref="MetricsType"/> monitoring an aspect of runtime performance
  /// </summary>
  public class MetricField
  {
    public MetricField(MapTypeKey key, MetricsType declaringType, FieldInfo declaringField, Metric metric)
    {
      Key = key;
      DeclaringType = declaringType;
      DeclaringField = declaringField;
      Metric = metric;
    }

    public readonly MapTypeKey Key;
    public readonly MetricsType DeclaringType;
    public readonly FieldInfo DeclaringField;
    public readonly Metric Metric;

    public override string ToString() => Key.ToString();
  }
}