using System;
using System.Diagnostics;

namespace Totem.Metrics
{
  /// <summary>
  /// Measures the running time for a process
  /// </summary>
  public class Duration : Metric<double>
  {
    public void Append(double seconds, MetricPath path = default(MetricPath)) =>
      AppendWrite(seconds, path);

    public void Append(TimeSpan duration, MetricPath path = default(MetricPath)) =>
      AppendWrite(duration.TotalSeconds, path);

    public IDisposable Measure(MetricPath path = default(MetricPath))
    {
      var stopwatch = Stopwatch.StartNew();

      return Disposal.Of(() => Append(stopwatch.Elapsed, path));
    }
  }
}