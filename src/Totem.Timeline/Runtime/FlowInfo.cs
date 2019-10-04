namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// The state of a flow record in the database
  /// </summary>
  public abstract class FlowInfo
  {
    internal FlowInfo()
    {}

    /// <summary>
    /// The flow has no routes or checkpoints
    /// </summary>
    public sealed class NotFound : FlowInfo
    {}

    /// <summary>
    /// The flow encountered an error and is not available
    /// </summary>
    public sealed class Stopped : FlowInfo
    {
      public Stopped(TimelinePosition position, string error)
      {
        Position = position;
        Error = error;
      }

      public readonly TimelinePosition Position;
      public readonly string Error;
    }

    /// <summary>
    /// The flow is present and available
    /// </summary>
    public sealed class Loaded : FlowInfo
    {
      public Loaded(Flow flow)
      {
        Flow = flow;
      }

      public readonly Flow Flow;
    }
  }
}