using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// Routes timeline points to a type of flow
  /// </summary>
  internal sealed class FlowRouter : PushScope
  {
    private readonly ConcurrentDictionary<Id, IFlowScope> _scopesById = new ConcurrentDictionary<Id, IFlowScope>();
    private readonly FlowType _flow;
    private readonly ITimelineScope _timeline;

    internal FlowRouter(FlowType flow, ITimelineScope timeline)
    {
      _flow = flow;
      _timeline = timeline;
    }

    public override Text ToText()
    {
      return $"{_flow} ({Text.Count(_scopesById.Count, "scope")})";
    }

    protected override void Push()
    {
      foreach(var flow in GetOrAddScopes())
      {
        flow.Push(Point);
      }
    }

    private IEnumerable<IFlowScope> GetOrAddScopes()
    {
      return
        from id in _flow.CallRoute(Point)
        select _scopesById.GetOrAdd(id, AddScope);
    }

    private IFlowScope AddScope(Id id)
    {
      var scope = OpenScope(id);

      var connection = scope.Connect(this);

      RemoveWhenDone(scope, connection);

      return scope;
    }

    private IFlowScope OpenScope(Id id)
    {
      return _timeline.OpenFlowScope(new FlowKey(_flow, id));
    }

    private void RemoveWhenDone(IFlowScope scope, IDisposable connection)
    {
      scope.Task.ContinueWith(_ =>
      {
        IFlowScope removed;

        _scopesById.TryRemove(scope.Key.Id, out removed);

        connection.Dispose();
      },
      State.CancellationToken);
    }
  }
}