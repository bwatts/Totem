using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The set of pending requests in a runtime
  /// </summary>
  internal sealed class TimelineRequestSet : Connection
  {
    private readonly ConcurrentDictionary<Id, TimelineRequest> _requestsById = new ConcurrentDictionary<Id, TimelineRequest>();
    private readonly ITimelineScope _scope;

    internal TimelineRequestSet(ITimelineScope scope)
    {
      _scope = scope;
    }

    protected override void Close()
    {
      base.Close();

      _requestsById.Clear();
    }

    public void Push(TimelinePoint point)
    {
      var requestId = Flow.Traits.RequestId.Get(point.Event);

      if(requestId.IsAssigned)
      {
        TimelineRequest request;

        if(_requestsById.TryGetValue(requestId, out request))
        {
          request.Push(point);
        }
      }
    }

    internal async Task<T> MakeRequest<T>(Id id) where T : Request
    {
      CheckUniqueRequestId(id);

      var request = AddRequest<T>(id);

      try
      {
        return await request.Task;
      }
      finally
      {
        RemoveRequest(id);
      }
    }

    private void CheckUniqueRequestId(Id id)
    {
      if(_requestsById.ContainsKey(id))
      {
        Log.Warning("[timeline] Request {Id} is already in progress", id);
      }
    }

    private TimelineRequest<T> AddRequest<T>(Id id) where T : Request
    {
      var request = CreateRequest<T>(id);

      _requestsById[id] = request;

      // No need to track the connection - the request closes when its task completes
      request.Connect(this);

      return request;
    }

    private TimelineRequest<T> CreateRequest<T>(Id id) where T : Request
    {
      return new TimelineRequest<T>(OpenScope<T>(id));
    }

    private IFlowScope OpenScope<T>(Id id)
    {
      var type = Runtime.GetRequest(typeof(T));

			IFlowScope scope;

			Expect(
				_scope.TryOpenFlowScope(type, new TimelineRoute(id), out scope),
				Text.Of("Failed to open scope for request \"{0}\" of type {1}", id, type));

			return scope;
    }

    private void RemoveRequest(Id id)
    {
      TimelineRequest request;

      _requestsById.TryRemove(id, out request);
    }
  }
}