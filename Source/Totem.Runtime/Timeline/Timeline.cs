using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// The chronological context of a domain
	/// </summary>
  /// <remarks>
  /// This is a naive but working implementation of domain + persistence
  /// </remarks>
	public sealed class Timeline : ITimeline
	{
		private readonly ITimelineDb _db;
		private readonly ITimelineScope _scope;

		public Timeline(ITimelineDb db, ITimelineScope scope)
		{
			_db = db;
			_scope = scope;
		}

		public void Write(TimelinePosition cause, Many<Event> events)
		{
			var points = _db.Write(cause, events);

			foreach(var point in points)
			{
				_scope.Push(point);
			}
		}

		public Task<TFlow> MakeRequest<TFlow>(Event e) where TFlow : Request
		{
			var requestId = Flow.Traits.EnsureRequestId(e);

			var requestTask = _scope.MakeRequest<TFlow>(requestId);

			Write(TimelinePosition.None, Many.Of(e));

			return requestTask;
		}
	}
}