namespace Totem.Metrics
{
  /// <summary>
  /// Measures the active and idle time for a process
  /// </summary>
  public class Activity : Metric<bool>
  {
    public void Set(bool active, MetricPath path = default(MetricPath)) =>
      AppendWrite(active, path);

    public void SetActive(MetricPath path = default(MetricPath)) =>
      AppendWrite(true, path);

    public void SetInactive(MetricPath path = default(MetricPath)) =>
      AppendWrite(false, path);
  }
}