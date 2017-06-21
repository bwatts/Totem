using System;
using System.Threading.Tasks;

namespace Totem.Metrics
{
  /// <summary>
  /// Describes the storage of writes to metrics
  /// </summary>
  public interface IMonitorDb
  {
    Task PushWrites(Action<IMonitor> writes);
  }
}