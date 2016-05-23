using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Totem.Runtime.Json;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A database persisting data pertaining to flows in the local runtime
	/// </summary>
	public sealed class LocalFlowDb : Notion, IFlowDb, IViewDb
	{
		private readonly ConcurrentDictionary<FlowKey, Flow> _flowsByKey = new ConcurrentDictionary<FlowKey, Flow>();
		private readonly LocalTimelineDb _timelineDb;
		private readonly Lazy<ITimelineScope> _timelineScope;

		public LocalFlowDb(LocalTimelineDb timelineDb, Lazy<ITimelineScope> timelineScope)
		{
			_timelineDb = timelineDb;
			_timelineScope = timelineScope;

			// Defer creation of the timeline scope to avoid a circular reference
		}

		public ClaimsPrincipal ReadPrincipal(TimelinePoint point)
		{
			return new ClaimsPrincipal();
		}

		public bool TryReadFlow(FlowType type, TimelineRoute route, out Flow flow)
		{
			var key = type.CreateKey(route.Id);

			if(!_flowsByKey.TryGetValue(key, out flow)
				&& (route.IsFirst || type.IsSingleInstance || type.IsRequest))
			{
				flow = type.New();

				Flow.Initialize(flow, key);

				if(!type.IsRequest)
				{
					_flowsByKey[key] = flow;
				}
			}

			return flow != null;
		}

		//
		// Write call
		//

		public void WriteCall(WhenCall call)
		{
			if(!call.Flow.Type.IsRequest && call is TopicWhenCall)
			{
				WriteTopicCall(call as TopicWhenCall);
			}
		}

		private void WriteTopicCall(TopicWhenCall call)
		{
			if(call.Flow.Done)
			{
				Flow removedFlow;

				_flowsByKey.TryRemove(call.Flow.Key, out removedFlow);
			}

			foreach(var newPoint in _timelineDb.Write(
				call.Point.Position,
				call.RetrieveNewEvents()))
			{
				_timelineScope.Value.Push(newPoint);
			}
		}

		//
		// Write error
		//

		public void WriteError(FlowKey key, TimelinePoint point, Exception error)
		{
			var stopped = new FlowStopped(key.Type.Key, key.Id, error.ToString());

			_timelineScope.Value.Push(WriteStopped(point, stopped));
		}

		private TimelinePoint WriteStopped(TimelinePoint point, FlowStopped stopped)
		{
			Flow.Traits.ForwardRequestId(point.Event, stopped);

			return _timelineDb.Write(point.Position, stopped);
		}

		//
		// Views
		//

		public ViewSnapshot<View> ReadSnapshot(Type type, Id id, TimelinePosition checkpoint)
		{
			return ReadView(type, id, checkpoint, view => (View) view);
		}

		public ViewSnapshot<T> ReadSnapshot<T>(Id id, TimelinePosition checkpoint) where T : View
		{
			return ReadView(typeof(T), id, checkpoint, view => (T) view);
		}

		public ViewSnapshot<string> ReadJsonSnapshot(Type type, Id id, TimelinePosition checkpoint)
		{
			return ReadView(type, id, checkpoint, view => JsonFormat.Text.Serialize(view).ToString());
		}

		private ViewSnapshot<T> ReadView<T>(Type type, Id id, TimelinePosition checkpoint, Func<Flow, T> selectContent)
		{
			var key = Runtime.GetView(type).CreateKey(id);

			Flow view;

			if(!_flowsByKey.TryGetValue(key, out view))
			{
				return ViewSnapshot<T>.OfNotFound(key);
			}

			ExpectNot(view.Done, "View is done and marked for removal: " + key.ToText());

			return view.Checkpoint == checkpoint
				? ViewSnapshot<T>.OfNotModified(key, checkpoint)
				: ViewSnapshot<T>.OfContent(key, view.Checkpoint, selectContent(view));
		}
	}
}