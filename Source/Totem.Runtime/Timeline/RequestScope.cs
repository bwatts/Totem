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
  internal sealed class RequestScope : FlowScope
  {
    readonly Client _client;

    internal RequestScope(ILifetimeScope lifetime, TimelineScope timeline, Request request, Client client)
      : base(lifetime, timeline, request)
    {
      _client = client;
    }

    internal Request Request => (Request) Flow;

    internal void PushError(Exception error)
    {
      CompleteTask(error);
    }

    internal async Task<Event> CallStart()
    {
      using(var lifetime = Lifetime.BeginCallScope())
      {
        var startEvent = await CallStart(lifetime);

        SetRequestId(startEvent);

        SetClientId(startEvent);

        return startEvent;
      }
    }

    Task<Event> CallStart(ILifetimeScope scope)
    {
      var call = new RequestStartCall(_client, scope.Resolve<IDependencySource>());

      return call.Make(Request);
    }

    void SetRequestId(Event startEvent)
    {
      Flow.Traits.RequestId.Set(startEvent, Flow.Id);
    }

    void SetClientId(Event startEvent)
    {
      Flow.Traits.ClientId.Set(startEvent, _client.Id);
    }
  }
}