using Totem.Runtime;

namespace Totem.Metrics
{
  /// <summary>
  /// Describes a monitor of aspects of runtime performance
  /// </summary>
  public interface IMonitor
  {
    void AppendWrite<T>(MetricWritten<T> write);
  }
}