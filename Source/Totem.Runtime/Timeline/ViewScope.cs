using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The scope of a view's activity on the timeline
  /// </summary>
  public sealed class ViewScope : FlowScope<View>
  {
    readonly IViewExchange _viewExchange;

    public ViewScope(ILifetimeScope lifetime, TimelineScope timeline, View instance, IViewExchange viewExchange)
      : base(lifetime, timeline, instance)
    {
      _viewExchange = viewExchange;
    }

    protected override async Task PushPoint()
    {
      try
      {
        await CallWhen();

        PushUpdate();

        CompleteTaskIfDone();
      }
      catch(Exception error)
      {
        PushStopped(error);
      }
    }

    async Task CallWhen()
    {
      using(var scope = BeginCallScope())
      {
        var call = new FlowCall.When(
          Point,
          GetFlowEvent(),
          scope.Resolve<IDependencySource>(),
          ReadPrincipal(),
          State.CancellationToken);

        await call.Make(Instance);

        PushWhen(call);
      }
    }

    void PushUpdate()
    {
      try
      {
        _viewExchange.PushUpdate(Instance);
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] Failed to push update of view {View}", Key);
      }
    }
  }
}