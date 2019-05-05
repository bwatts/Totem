using Totem.Runtime;

namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// The checkpoint, routes, and schedule needed to resume the timeline subscription
  /// </summary>
  public class ResumeInfo
  {
    public ResumeInfo(IConnectable subscription)
    {
      Checkpoint = TimelinePosition.None;
      Routes = new Many<FlowKey>();
      Schedule = new Many<TimelinePoint>();
      Subscription = subscription;
    }

    public ResumeInfo(TimelinePosition checkpoint, Many<FlowKey> routes, Many<TimelinePoint> schedule, IConnectable subscription)
    {
      Checkpoint = checkpoint;
      Routes = routes;
      Schedule = schedule;
      Subscription = subscription;
    }

    public readonly TimelinePosition Checkpoint;
    public readonly Many<FlowKey> Routes;
    public readonly Many<TimelinePoint> Schedule;
    public readonly IConnectable Subscription;
  }
}