using System.Reflection;
using Totem.Metrics;

namespace Totem.Runtime.Map
{
  /// <summary>
	/// A .NET type declaring a set of related metrics
	/// </summary>
  public class RuntimeMetricType : RuntimeType
  {
    internal RuntimeMetricType(RuntimeTypeRef type) : base(type)
    {}

    public readonly RuntimeMetricSet Metrics = new RuntimeMetricSet();

    internal RuntimeMetric RegisterMetric(FieldInfo field, Metric declaration)
    {
      var key = RuntimeTypeKey.From(Key.Region, field.Name);

      var metric = new RuntimeMetric(key, this, field, declaration);

      Metrics.Register(metric);

      return metric;
    }
  }
}