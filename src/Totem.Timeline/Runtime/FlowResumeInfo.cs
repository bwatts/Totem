namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// A flow's current state and the points to resume its activity
  /// </summary>
  public abstract class FlowResumeInfo
  {
    internal FlowResumeInfo()
    {}

    public class NotFound : FlowResumeInfo
    {}

    public class Stopped : FlowResumeInfo
    {
      public Stopped(TimelinePosition position, string error)
      {
        Position = position;
        Error = error;
      }

      public readonly TimelinePosition Position;
      public readonly string Error;
    }

    public class Loaded : FlowResumeInfo
    {
      public Loaded(Flow flow, Many<TimelinePoint> points)
      {
        Flow = flow;
        Points = points;
      }

      public readonly Flow Flow;
      public readonly Many<TimelinePoint> Points;
    }
  }
}