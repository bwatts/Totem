using System.Threading.Tasks;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// An event observed by a <see cref="Topic"/>
  /// </summary>
  public sealed class TopicObservation : FlowObservation
  {
    internal TopicObservation(
      FlowType flowType,
      EventType eventType,
      FlowRoute route,
      FlowMethodSet<FlowGiven> given,
      FlowMethodSet<TopicWhen> when)
      : base(flowType, eventType, route, given)
    {
      When = when;
    }

    public readonly FlowMethodSet<TopicWhen> When;

    protected override bool CanRoute(Event e, bool scheduled) =>
      EventType.IsTypeOf(e) && (HasGiven(scheduled) || HasWhen(scheduled));

    public bool HasWhen(bool scheduled) =>
      When.HasMethod(scheduled);

    public async Task CallWhen(Topic topic, FlowCall.When call)
    {
      foreach(var whenMethod in When.GetMethods(call.Point.Scheduled))
      {
        await whenMethod.Call(topic, call.Point.Event, call.Services);
      }
    }
  }
}