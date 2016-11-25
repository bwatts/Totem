using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The scope of a request's activity on the timeline
  /// </summary>
  internal sealed class RequestScope : FlowScope
  {
    readonly ILifetimeScope _lifetime;

    internal RequestScope(ILifetimeScope lifetime, TimelineScope timeline, FlowRoute initialRoute)
      : base(timeline, initialRoute)
    {
      _lifetime = lifetime;
    }

    public void PushError(Exception error)
    {
      CompleteTask(error);
    }

    protected override async Task PushPoint()
    {
      try
      {
        await CallWhen();
      }
      catch(Exception error)
      {
        PushError(error);
      }

      if(Flow.Context.Done)
      {
        CompleteTask();
      }
    }

    async Task CallWhen()
    {
      var flowEvent = GetFlowEvent();

      if(flowEvent != null)
      {
        Log.Verbose("[timeline] {Position:l} => {Flow:l}", Point.Position, Key);

        using(var scope = _lifetime.BeginCallScope())
        {
          var call = new FlowCall.When(
            Point,
            flowEvent,
            scope.Resolve<IDependencySource>(),
            Timeline.ReadPrincipal(Point),
            State.CancellationToken);

          await call.Make(Flow);
        }
      }
    }

    FlowEvent GetFlowEvent()
    {
      return Key.Type.Events.Get(Point.Event, strict: false);
    }
  }
}