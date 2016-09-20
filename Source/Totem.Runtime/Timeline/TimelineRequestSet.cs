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
    private readonly TimelineScope _timeline;

    internal TimelineRequestSet(TimelineScope timeline)
    {
      _timeline = timeline;
    }

    public void Push(TimelineMessage message)
    {
      var requestId = Flow.Traits.RequestId.Get(message.Point.Event);

      if(requestId.IsAssigned)
      {
        TimelineRequest request;

        if(_requestsById.TryGetValue(requestId, out request))
        {
          request.Push(message);
        }
      }
    }

    internal void TryPushError(Id requestId, Exception error)
    {
      TimelineRequest request;

      if(_requestsById.TryGetValue(requestId, out request))
      {
        request.PushError(error);
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
			var request = _timeline.CreateRequest<T>(id);

			_requestsById[id] = request;

      // No need to track the connection - the request closes when its task completes
      request.Connect(this);

      return request;
    }

    private void RemoveRequest(Id id)
    {
      TimelineRequest request;

      _requestsById.TryRemove(id, out request);
    }
  }
}