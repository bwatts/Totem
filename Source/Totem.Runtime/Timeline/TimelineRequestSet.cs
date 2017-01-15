using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The set of pending requests in a runtime
  /// </summary>
  internal sealed class TimelineRequestSet : Connection
  {
    readonly ConcurrentDictionary<Id, RequestScope> _requestsById = new ConcurrentDictionary<Id, RequestScope>();
    readonly TimelineScope _timeline;

    internal TimelineRequestSet(TimelineScope timeline)
    {
      _timeline = timeline;
    }

    internal void Push(TimelineMessage message)
    {
      var requestId = message.Point.RequestId;

      RequestScope request;

      if(requestId.IsAssigned && _requestsById.TryGetValue(requestId, out request))
      {
        var route = new FlowRoute(request.Key, first: false, when: true, given: false, then: false);

        request.Push(new FlowPoint(route, message.Point));
      }
    }

    internal void TryPushError(TimelinePoint point, Exception error)
    {
      var requestId = point.RequestId;

      RequestScope request;

      if(requestId.IsAssigned && _requestsById.TryGetValue(requestId, out request))
      {
        request.PushError(error);
      }
    }

    internal async Task Execute(RequestScope scope)
    {
      var id = scope.Request.Id;

      CheckUnique(id);

      AddRequest(id, scope);

      try
      {
        await scope.Task;
      }
      finally
      {
        RemoveRequest(id);
      }
    }

    void CheckUnique(Id id)
    {
      if(_requestsById.ContainsKey(id))
      {
        Log.Warning("[timeline] Request {Id} is already in progress", id);
      }
    }

    void AddRequest(Id id, RequestScope scope)
    {
			_requestsById[id] = scope;

      scope.Connect(this);
    }

    void RemoveRequest(Id id)
    {
      RequestScope scope;

      _requestsById.TryRemove(id, out scope);
    }
  }
}