using System.Collections.Generic;
using Totem.Metrics;

namespace Totem.Runtime.Map
{
  /// <summary>
	/// A set of metrics, indexed by key
	/// </summary>
  public class RuntimeMetricSet : RuntimeSet<RuntimeTypeKey, RuntimeMetric>
  {
    readonly Dictionary<Metric, RuntimeMetric> _metricsByDeclaration = new Dictionary<Metric, RuntimeMetric>();

    internal override RuntimeTypeKey GetKey(RuntimeMetric value) =>
      value.Key;

    internal override void RegisterByOtherKeys(RuntimeMetric value) =>
      _metricsByDeclaration.Add(value.Declaration, value);

    public RuntimeMetric Get(Metric declaration, bool strict = true)
    {
      RuntimeMetric metric;

      if(!_metricsByDeclaration.TryGetValue(declaration, out metric) && strict)
      {
        throw new KeyNotFoundException($"No metric found for declaration of type {declaration.GetType()}");
      }

      return metric;
    }
  }
}