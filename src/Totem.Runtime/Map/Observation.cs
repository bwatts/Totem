using Totem.Core;

namespace Totem.Map;

public class Observation : IMapTypeKeyed
{
    internal Observation(
        ObserverType observer,
        EventType e,
        ObserverRouteMethod? route,
        GivenMethod? given,
        ObserverWhenMethod? when)
    {
        Observer = observer;
        Event = e;
        Route = route;
        Given = given;
        When = when;
    }

    public ObserverType Observer { get; }
    public EventType Event { get; }
    public ObserverRouteMethod? Route { get; }
    public GivenMethod? Given { get; }
    public ObserverWhenMethod? When { get; }

    Type ITypeKeyed.TypeKey => Event.DeclaredType;
    MapType IMapTypeKeyed.MapTypeKey => Event;

    public override string ToString() =>
        Event.DeclaredType.ToString();

    internal IEnumerable<ItemKey> CallRoute(IEventContext<IEvent> context) =>
        Observer.SingleInstanceId is not null
            ? new[] { Observer.CallSingleInstanceRoute() }
            : Route!.Call(context.Event);
}
