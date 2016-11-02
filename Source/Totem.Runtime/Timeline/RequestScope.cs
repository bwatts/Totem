using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The scope of a request's activity on the timeline
  /// </summary>
  public sealed class RequestScope<T> : FlowScope<T>, IRequestScope where T : Request
  {
    public RequestScope(ILifetimeScope lifetime, TimelineScope timeline, T instance)
      : base(lifetime, timeline, instance)
    {}

    void IRequestScope.PushError(Exception error)
    {
      CompleteTask(error);
    }

    protected override async Task PushPoint()
    {
      try
      {
        await CallWhen();

        CompleteTaskIfDone();
      }
      catch(Exception error)
      {
        CompleteTask(error);
      }
    }

    async Task CallWhen()
    {
      var flowEvent = GetFlowEvent(strict: false);

      if(flowEvent != null)
      {
        using(var scope = BeginCallScope())
        {
          var call = new FlowCall.When(
            Point,
            flowEvent,
            scope.Resolve<IDependencySource>(),
            ReadPrincipal(),
            State.CancellationToken);

          await call.Make(Instance);
        }
      }
    }
  }
}