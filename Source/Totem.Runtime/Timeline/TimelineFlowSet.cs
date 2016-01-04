using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Totem.Runtime.Map;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The set of flows acting in a runtime
  /// </summary>
  internal sealed class TimelineFlowSet : PushScope
  {
    private readonly Dictionary<EventType, EventRouter> _eventRoutersByType = new Dictionary<EventType, EventRouter>();
    private readonly ITimelineScope _scope;

    internal TimelineFlowSet(ITimelineScope scope)
    {
      _scope = scope;
    }

    protected override void Open()
    {
      var packages = ReadPackages();

      AddEventRouters(packages);

      AddFlowRouters(packages);

      base.Open();
    }

    protected override void Close()
    {
      base.Close();

      _eventRoutersByType.Clear();
    }

    protected override void Push()
    {
      EventRouter eventRouter;

      if(!_eventRoutersByType.TryGetValue(Point.EventType, out eventRouter))
      {
        throw new InvalidOperationException($"Cannot push to unknown event type {Point.EventType}");
      }

      eventRouter.Push(Point);
    }

    private List<RuntimePackage> ReadPackages()
    {
      return Runtime.Regions.SelectMany(region => region.Packages).ToList();
    }

    private void AddEventRouters(List<RuntimePackage> packages)
    {
      foreach(var e in
        from package in packages
        from e in package.Events
        select e)
      {
        _eventRoutersByType.Add(e, new EventRouter(e));
      }
    }

    private void AddFlowRouters(List<RuntimePackage> packages)
    {
      // Requests are handled by a separate pipeline defined by TimelineRequestSet

      foreach(var flow in
        from package in packages
        from flow in package.Flows
        where !flow.IsRequest
        select new
        {
          Router = new FlowRouter(flow, _scope),
          Events = flow.Events.Select(e => e.EventType)
        })
      {
        foreach(var e in flow.Events)
        {
          _eventRoutersByType[e].RegisterFlow(flow.Router);
        }

        Track(flow.Router);
      }
    }
  }
}