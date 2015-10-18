using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// The chronological context of a domain
	/// </summary>
	public sealed class Timeline : ITimeline
	{
		private readonly ITimelineDb _db;
		private readonly ITimelineHost _host;

		public Timeline(ITimelineDb db, ITimelineHost host)
		{
			_db = db;
			_host = host;
		}

		public void Append(TimelinePosition cause, Many<Event> events)
		{
			var points = _db.Append(cause, events);

			foreach(var point in points)
			{
				_host.Push(point);
			}
		}

		public Task<TFlow> MakeRequest<TFlow>(TimelinePosition cause, Event e) where TFlow : Request
		{
			var requestId = Flow.Traits.EnsureRequestId(e);

			var requestTask = _host.MakeRequest<TFlow>(requestId);

			Append(cause, Many.Of(e));

			return requestTask;
		}
	}
}