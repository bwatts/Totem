using System.Collections.Generic;
using System.Threading.Tasks;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// An event type observed by a flow
  /// </summary>
  public class FlowObservation
  {
    public FlowObservation(FlowType flowType, EventType eventType, FlowRoute route, FlowMethodSet<FlowWhen> when)
    {
      FlowType = flowType;
      EventType = eventType;
      Route = route;
      When = when;
    }

    public readonly FlowType FlowType;
    public readonly EventType EventType;
    public readonly FlowRoute Route;
    public readonly FlowMethodSet<FlowWhen> When;

    public bool HasRoute => Route != null;
    public bool CanBeFirst => Route == null || Route.First;

    public override string ToString() =>
      $"{EventType} => {FlowType}";

    public IEnumerable<FlowKey> GetRoutes(Event e, bool scheduled)
    {
      if(!CanRoute(e, scheduled))
      {
        yield break;
      }
      else if(HasRoute)
      {
        foreach(var id in Route.Call(e))
        {
          yield return FlowKey.From(FlowType, id);
        }
      }
      else
      {
        yield return FlowKey.From(FlowType);
      }
    }

    protected virtual bool CanRoute(Event e, bool scheduled) =>
      EventType.IsTypeOf(e) && HasWhen(scheduled);

    public bool HasWhen(bool scheduled) =>
      When.HasMethod(scheduled);

    public bool HasWhen() =>
      When.HasMethod(true) || When.HasMethod(false);

    public async Task CallWhen(Flow flow, FlowCall.When call)
    {
      foreach(var whenMethod in When.GetMethods(call.Point.Scheduled))
      {
        await whenMethod.Call(flow, call.Point.Event, call.Services);
      }
    }
  }
}