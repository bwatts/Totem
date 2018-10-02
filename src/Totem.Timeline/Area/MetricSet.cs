using System.Collections.Generic;
using Totem.Runtime.Metrics;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A set of declared metrics, indexed by key
  /// </summary>
  public class MetricSet : MapSet<MapTypeKey, MetricField>
  {
    readonly Dictionary<Metric, MetricField> _fieldsByMetric = new Dictionary<Metric, MetricField>();

    internal override MapTypeKey GetKey(MetricField value) =>
      value.Key;

    internal override void DeclareByOtherKeys(MetricField value) =>
      _fieldsByMetric.Add(value.Metric, value);

    public MetricField Get(Metric metric, bool strict = true)
    {
      if(!_fieldsByMetric.TryGetValue(metric, out var field) && strict)
      {
        throw new KeyNotFoundException($"No field found for metric {metric}");
      }

      return field;
    }
  }
}