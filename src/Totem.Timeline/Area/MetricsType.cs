namespace Totem.Timeline.Area
{
  /// <summary>
  /// A .NET type declaring a set of related metrics
  /// </summary>
  public class MetricsType : MapType
  {
    public MetricsType(MapTypeInfo type) : base(type)
    {}

    public readonly MetricSet Metrics = new MetricSet();
  }
}