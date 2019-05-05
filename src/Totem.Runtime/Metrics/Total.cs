namespace Totem.Runtime.Metrics
{
  /// <summary>
  /// Measures a discrete increasing quantity
  /// </summary>
  public class Total : Metric<int>
  {
    public void Increment(MetricPath path = default(MetricPath)) =>
      AppendWrite(current => current + 1, path);

    public void IncrementBy(int count, MetricPath path = default(MetricPath)) =>
      AppendWrite(current => current + count, path);
  }
}