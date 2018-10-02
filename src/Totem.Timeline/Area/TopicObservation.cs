namespace Totem.Timeline.Area
{
  /// <summary>
  /// An event observed by a <see cref="Topic"/>
  /// </summary>
  public class TopicObservation : FlowObservation
  {
    public TopicObservation(
      FlowType flowType,
      EventType eventType,
      FlowRoute route,
      FlowMethodSet<FlowWhen> when,
      FlowMethodSet<TopicGiven> given)
      : base(flowType, eventType, route, when)
    {
      Given = given;
    }

    public readonly FlowMethodSet<TopicGiven> Given;

    protected override bool CanRoute(Event e, bool scheduled) =>
      EventType.IsTypeOf(e) && (HasWhen(scheduled) || HasGiven(scheduled));

    public bool HasGiven(bool scheduled) =>
      Given.HasMethod(scheduled);

    public bool HasGiven() =>
      Given.HasMethod(true) || Given.HasMethod(false);

    public void CallGiven(Topic topic, FlowCall.Given call)
    {
      foreach(var givenMethod in Given.GetMethods(call.Point.Scheduled))
      {
        givenMethod.Call(topic, call.Point.Event);
      }
    }
  }
}