using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Totem.Runtime;
using Totem.Timeline.Runtime;

namespace Totem.Timeline
{
  /// <summary>
  /// The context of a flow's presence on the timeline
  /// </summary>
  public sealed class FlowContext
  {
    readonly Flow _flow;
    User _user;

    FlowContext(Flow flow, FlowKey key)
    {
      _flow = flow;
      Key = key;
      IsNew = true;
    }

    FlowContext(
      Flow flow,
      FlowKey key,
      TimelinePosition checkpointPosition,
      TimelinePosition errorPosition)
      : this(flow, key)
    {
      CheckpointPosition = checkpointPosition;
      ErrorPosition = errorPosition;
      IsNew = false;
    }

    public readonly FlowKey Key;
    public TimelinePosition CheckpointPosition { get; private set; }
    public TimelinePosition ErrorPosition { get; private set; }
    public string ErrorMessage { get; private set; }
    public FlowCall Call { get; private set; }
    public bool IsNew { get; private set; }
    public bool IsDone { get; private set; }

    public override string ToString() =>
      Key.ToString();

    internal void CallGiven(FlowCall.Given call, bool advanceCheckpoint = false)
    {
      try
      {
        Call = call;

        call.Make(_flow);

        if(advanceCheckpoint)
        {
          AdvanceCheckpoint();
        }
      }
      finally
      {
        Call = null;
      }
    }

    internal async Task CallWhen(FlowCall.When call)
    {
      try
      {
        Call = call;

        await call.Make((Topic) _flow);

        AdvanceCheckpoint();
      }
      finally
      {
        Call = null;
      }
    }

    void AdvanceCheckpoint() =>
      CheckpointPosition = Call.Point.Position;

    internal void SetError(TimelinePosition position, string message)
    {
      ErrorPosition = position;
      ErrorMessage = message;
    }

    internal void SetNotNew() =>
      IsNew = false;

    internal void SetDone() =>
      IsDone = true;

    internal async Task<User> ReadUser()
    {
      var whenCall = Call as FlowCall.When;

      Expect.That(whenCall).IsNotNull("Flow is not making a When call. This may indicate an asynchronous operation was not awaited during a prior When call.");

      if(_user == null)
      {
        var userDb = whenCall.Services.GetService<IUserDb>();

        _user = userDb == null
          ? new User()
          : await userDb.Authenticate(Call.Point.UserId);
      }

      return _user;
    }

    //
    // Bind
    //

    public static void Bind(Flow flow, FlowKey key)
    {
      Expect.That(flow.Context).IsNull("The flow is already bound to a context");

      flow.Context = new FlowContext(flow, key);
      flow.Id = key.Id;
    }

    public static void Bind(
      Flow flow,
      FlowKey key,
      TimelinePosition checkpointPosition,
      TimelinePosition errorPosition)
    {
      Expect.That(flow.Context).IsNull("The flow is already bound to a context");

      flow.Context = new FlowContext(flow, key, checkpointPosition, errorPosition);
      flow.Id = key.Id;
    }
  }
}