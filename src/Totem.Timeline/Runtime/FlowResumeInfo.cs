namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// The state of a flow record in the database and the points to resume its activity
  /// </summary>
  public class FlowResumeInfo
  {
    public FlowResumeInfo(Flow flow, Many<TimelinePoint> points)
    {
      Flow = flow;
      Points = points;
    }

    public readonly Flow Flow;
    public readonly Many<TimelinePoint> Points;
  }
}