using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Totem.Runtime;

namespace Totem.Tracking
{
  /// <summary>
  /// An index of tracked timeline events
  /// </summary>
	public sealed class EventIndex : Connection, IEventIndex
	{
		readonly ConcurrentQueue<Action<IEventIndex>> _writes = new ConcurrentQueue<Action<IEventIndex>>();
		readonly IEventIndexDb _db;
		readonly TimeSpan _pushRate;
		bool _tracking;

		public EventIndex(IEventIndexDb db, TimeSpan pushRate)
		{
			_db = db;
			_pushRate = pushRate;
		}

		protected override async void Open() => await StartTracking();
		protected override void Close() => _tracking = false;

		async Task StartTracking()
		{
			_tracking = true;

			while(_tracking)
			{
				await Task.Delay(_pushRate);
				await PushWrites();
			}
		}

		public void Index(TrackedEvent e) =>
			_writes.Enqueue(tracker => tracker.Index(e));

		async Task PushWrites()
		{
			try
			{
				await PushBatch();
			}
			catch(Exception error)
			{
				Log.Error(error.Message);
			}
		}

		Task PushBatch()
		{
			return _db.PushWrites(batch =>
			{
        Action<IEventIndex> write;

        while(_writes.TryDequeue(out write))
				{
					write(batch);
				}
			});
		}
	}
}