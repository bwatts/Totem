using System.Collections.Generic;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// An event type observed by a flow
  /// </summary>
  public class FlowObservation
  {
    internal FlowObservation(FlowType flowType, EventType eventType, FlowRoute route, FlowMethodSet<FlowGiven> given)
    {
      FlowType = flowType;
      EventType = eventType;
      Route = route;
      Given = given;
    }

    public readonly FlowType FlowType;
    public readonly EventType EventType;
    public readonly FlowRoute Route;
    public readonly FlowMethodSet<FlowGiven> Given;

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
      EventType.IsTypeOf(e) && HasGiven(scheduled);

    public bool HasGiven(bool scheduled) =>
      Given.HasMethod(scheduled);

    public void CallGiven(Flow flow, FlowCall.Given call)
    {
      foreach(var givenMethod in Given.GetMethods(call.Point.Scheduled))
      {
        givenMethod.Call(flow, call.Point.Event);
      }
    }
  }
}