using System;

namespace Totem.Runtime.Metrics
{
  /// <summary>
  /// Measures a discrete fluctuating quantity
  /// </summary>
  public class LongCount : Metric<long>
  {
    public void Set(long count, MetricPath path = default(MetricPath)) =>
      AppendWrite(count, path);

    public void Increment(MetricPath path = default(MetricPath)) =>
      AppendWrite(current => current + 1, path);

    public void Decrement(MetricPath path = default(MetricPath)) =>
      AppendWrite(current => current - 1, path);

    public void IncrementBy(long count, MetricPath path = default(MetricPath)) =>
      AppendWrite(current => current + count, path);

    public void DecrementBy(long count, MetricPath path = default(MetricPath)) =>
      AppendWrite(current => current + count, path);

    public IDisposable IncrementDuring(MetricPath path = default(MetricPath))
    {
      Increment(path);

      return Disposal.Of(() => Decrement(path));
    }

    public IDisposable DecrementDuring(MetricPath path = default(MetricPath))
    {
      Decrement(path);

      return Disposal.Of(() => Increment(path));
    }

    public IDisposable IncrementDuringBy(long count, MetricPath path = default(MetricPath))
    {
      IncrementBy(count, path);

      return Disposal.Of(() => DecrementBy(count, path));
    }

    public IDisposable DecrementDuringBy(long count, MetricPath path = default(MetricPath))
    {
      DecrementBy(count, path);

      return Disposal.Of(() => IncrementBy(count, path));
    }
  }
}