using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Totem.Runtime.Json;

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

		//
		// Read instance
		//

		public Flow ReadInstance(FlowKey key)
		{
			return key.Type.IsRequest ? CreateInstance(key) : ReadOrCreateInstance(key);
		}

		private Flow CreateInstance(FlowKey key)
		{
			var flow = key.Type.New();

			Flow.Initialize(flow, key);

			return flow;
		}

		private Flow ReadOrCreateInstance(FlowKey key)
		{
			return _flowsByKey.GetOrAdd(key, _ => CreateInstance(key));
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

		public View Read(Type viewType, Id id, bool strict = true)
		{
			var runtimeType = Runtime.GetView(viewType, strict);

			if(runtimeType == null)
			{
				return null;
			}

			var key = runtimeType.CreateKey(id);

			Flow view;

			if(_flowsByKey.TryGetValue(key, out view))
			{
				ExpectNot(strict && view.Done, Text
					.Of("View is done and marked for removal: ")
					.Write(runtimeType)
					.WriteIf(id.IsAssigned, $"/{id}"));

				return (View) view;
			}

			ExpectNot(strict, Text
				.Of("View not found: ")
				.Write(runtimeType)
				.WriteIf(id.IsAssigned, $"/{id}"));

			return null;
		}

		public string ReadJson(Type viewType, Id id, bool strict = true)
		{
			var view = Read(viewType, id, strict);

			return view == null ? null : JsonFormat.Text.Serialize(view).ToString();
		}
	}
}