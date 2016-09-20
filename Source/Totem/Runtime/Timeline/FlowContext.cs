using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// The context of a flow's presence on the timeline
	/// </summary>
	public sealed class FlowContext : Notion
	{
		private FlowContext(FlowKey key)
		{
			Key = key;
		}

    private FlowContext(
      FlowKey key,
      TimelinePosition checkpointPosition,
      TimelinePosition snapshotPosition,
      TimelinePosition errorPosition)
		{
			Key = key;
			CheckpointPosition = checkpointPosition;
			SnapshotPosition = snapshotPosition;
      ErrorPosition = errorPosition;
      HasError = errorPosition.IsSome;
    }

		public readonly FlowKey Key;
    public FlowType Type => Key.Type;
		public TimelinePosition CheckpointPosition { get; private set; }
		public TimelinePosition SnapshotPosition { get; private set; }
    public TimelinePosition ErrorPosition { get; private set; }
    public bool HasError { get; private set; }
    public bool Done { get; private set; }

		public override Text ToText() => Key.ToText();

		internal void FinishCall(Flow flow, FlowCall call)
		{
      var topic = flow as Topic;

      if(topic == null)
      {
        CheckpointPosition = call.Point.Position;
        SnapshotPosition = call.Point.Position;
      }
      else
      {
        if(call is FlowCall.When || !call.Point.Route.When)
        {
          CheckpointPosition = call.Point.Position;

          if(topic.ShouldSnapshot())
          {
            SnapshotPosition = call.Point.Position;
          }
        }
      }
		}

    internal void SetError(TimelinePosition position)
    {
      ErrorPosition = position;
    }

    internal void SetDone()
		{
			Done = true;
		}

    //
    // Bind
    //

    public static void Bind(Flow flow, FlowKey key)
    {
      Expect(flow.Context).IsNull("The flow is already bound to a context");

      flow.Context = new FlowContext(key);
      flow.Id = key.Id;
    }

    public static void Bind(
      Flow flow,
      FlowKey key,
      TimelinePosition checkpointPosition,
      TimelinePosition snapshotPosition,
      TimelinePosition errorPosition)
    {
      Expect(flow.Context).IsNull("The flow is already bound to a context");

      flow.Context = new FlowContext(key, checkpointPosition, snapshotPosition, errorPosition);
      flow.Id = key.Id;
    }
  }
}