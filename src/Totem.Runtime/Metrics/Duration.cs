using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Totem.Runtime.Metrics
{
  /// <summary>
  /// Measures the running time for a process
  /// </summary>
  public class Duration : Metric<double>
  {
    readonly ConcurrentDictionary<MetricPath, Stopwatch> _stopwatches = new ConcurrentDictionary<MetricPath, Stopwatch>();

    public void Append(double seconds, MetricPath path = default(MetricPath)) =>
      AppendWrite(seconds, path);

    public void Append(TimeSpan duration, MetricPath path = default(MetricPath)) =>
      AppendWrite(duration.TotalSeconds, path);

    public IDisposable Measure(MetricPath path = default(MetricPath))
    {
      var stopwatch = Stopwatch.StartNew();

      return Disposal.Of(() => Append(stopwatch.Elapsed, path));
    }

    public void StartMeasuring(MetricPath path = default(MetricPath)) =>
      _stopwatches[path] = Stopwatch.StartNew();

    public bool StopMeasuring(MetricPath path = default(MetricPath))
    {
      Stopwatch stopwatch;

      if(_stopwatches.TryRemove(path, out stopwatch))
      {
        Append(stopwatch.Elapsed, path);

        return true;
      }

      return false;
    }
  }
}