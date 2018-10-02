using System;
using System.Collections.Concurrent;

namespace Totem.Runtime.Metrics
{
  /// <summary>
  /// A monitored aspect of runtime performance
  /// </summary>
  public class Metric : Notion
  {
    protected IMonitor Monitor
    {
      get { return Traits.Monitor.Get(this); }
      set { Traits.Monitor.Set(this, value); }
    }

    public new static class Traits
    {
      public static readonly Field<IMonitor> Monitor = Field.Declare(() => Monitor, new UninitializedMonitor());

      public static void InitializeMonitor(IMonitor monitor) =>
        Monitor.SetDefault(monitor);

      class UninitializedMonitor : IMonitor
      {
        public void AppendWrite<T>(MetricWritten<T> write)
        {}
      }
    }
  }

  /// <summary>
  /// A monitored aspect of runtime performance with a value of the specified type
  /// </summary>
  /// <typeparam name="T">The type of monitored value</typeparam>
  public abstract class Metric<T> : Metric
  {
    readonly ConcurrentDictionary<MetricPath, T> _valuesByPath = new ConcurrentDictionary<MetricPath, T>();

    protected void AppendWrite(T value, MetricPath path = default(MetricPath))
    {
      _valuesByPath[path] = value;

      AppendToMonitor(value, path);
    }

    protected void AppendWrite(Func<T, T> calculateValue, MetricPath path = default(MetricPath))
    {
      var value = _valuesByPath.AddOrUpdate(
        path,
        _ => calculateValue(default(T)),
        (_, current) => calculateValue(current));

      AppendToMonitor(value, path);
    }

    void AppendToMonitor(T value, MetricPath path) =>
      Monitor.AppendWrite(new MetricWritten<T>(Clock.Now, this, path, value));
  }
}