using System;
using Totem.Metrics;

namespace Totem.Runtime.Map
{
  /// <summary>
  /// A set of metrics monitoring runtime performance
  /// </summary>
  public class RuntimeMonitor
  {
    public readonly RuntimeTypeSet<RuntimeMetricType> MetricTypes = new RuntimeTypeSet<RuntimeMetricType>();
    public readonly RuntimeMetricSet Metrics = new RuntimeMetricSet();

    public RuntimeMetricType GetMetricType(RuntimeTypeKey key, bool strict = true) =>
      MetricTypes.Get(key, strict);

    public RuntimeMetricType GetMetricType(Type declaredType, bool strict = true) =>
      MetricTypes.Get(declaredType, strict);

    public RuntimeMetric GetMetric(RuntimeTypeKey key, bool strict = true) =>
      Metrics.Get(key, strict);

    public RuntimeMetric GetMetric(Metric declaration, bool strict = true) =>
      Metrics.Get(declaration, strict);
  }
}