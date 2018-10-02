using System;

namespace Totem.Runtime.Metrics
{
  /// <summary>
  /// Measures an arbitrary quantity
  /// </summary>
  public class Amount : Metric<double>
  {
    public void Set(double amount, MetricPath path = default(MetricPath)) =>
      AppendWrite(amount, path);

    public void Increment(MetricPath path = default(MetricPath)) =>
      AppendWrite(current => current + 1, path);

    public void Decrement(MetricPath path = default(MetricPath)) =>
      AppendWrite(current => current - 1, path);

    public void IncrementBy(double amount, MetricPath path = default(MetricPath)) =>
      AppendWrite(current => current + amount, path);

    public void DecrementBy(double amount, MetricPath path = default(MetricPath)) =>
      AppendWrite(current => current - amount, path);

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

    public IDisposable IncrementDuringBy(double amount, MetricPath path = default(MetricPath))
    {
      IncrementBy(amount, path);

      return Disposal.Of(() => DecrementBy(amount, path));
    }

    public IDisposable DecrementDuringBy(double amount, MetricPath path = default(MetricPath))
    {
      DecrementBy(amount, path);

      return Disposal.Of(() => IncrementBy(amount, path));
    }
  }
}