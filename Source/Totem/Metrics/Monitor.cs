using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Totem.Runtime;

namespace Totem.Metrics
{
  /// <summary>
  /// A monitor of runtime performance measurements
  /// </summary>
  public sealed class Monitor : Connection, IMonitor
  {
		readonly ConcurrentQueue<Action<IMonitor>> _writes = new ConcurrentQueue<Action<IMonitor>>();
		readonly IMonitorDb _db;
		readonly TimeSpan _pushRate;
		readonly int _errorThreshold;
		bool _monitoring;
		int _errorCount;

		public Monitor(IMonitorDb db, TimeSpan pushRate, int errorThreshold)
    {
			_db = db;
			_pushRate = pushRate;
			_errorThreshold = errorThreshold;
		}

		protected override void Open() => StartMonitoring();
		protected override void Close() => _monitoring = false;

		async Task StartMonitoring()
		{
			_monitoring = true;

			while(_monitoring)
			{
				await Task.Delay(_pushRate);

				await PushWrites();
			}
		}

    public void AppendWrite<T>(MetricWritten<T> write) =>
      _writes.Enqueue(monitor => monitor.AppendWrite(write));

    async Task PushWrites()
    {
      try
      {
        await PushBatch();

        _errorCount = 0;
      }
      catch(Exception error)
      {
        CheckErrorThreshold(error);
      }
    }

    Task PushBatch()
    {
      return _db.PushWrites(batch =>
      {
        Action<IMonitor> write;

        while(_writes.TryDequeue(out write))
        {
          write(batch);
        }
      });
    }

		void CheckErrorThreshold(Exception error)
		{
			if(_errorCount == _errorThreshold)
			{
				Log.Error(error, $"[monitor] Stopping: unable to push counter samples after error threshold of {_errorThreshold}");

				_monitoring = false;
			}
			else
			{
				_errorCount++;

				Log.Warning(error, $"[monitor] Unable to push counter samples after attempt {_errorCount}");
			}
		}
	}
}