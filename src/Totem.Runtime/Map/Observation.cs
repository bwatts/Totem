namespace Totem.Map;

public class Observation : IMapTypeKeyed
{
    internal Observation(EventType e, ObserverRouteMethod route, GivenMethod? given, ObserverWhenMethod? when)
    {
        Event = e;
        Route = route;
        Given = given;
        When = when;
    }

    public EventType Event { get; }
    public ObserverRouteMethod Route { get; }
    public GivenMethod? Given { get; }
    public ObserverWhenMethod? When { get; }

    Type ITypeKeyed.TypeKey => Event.DeclaredType;
    MapType IMapTypeKeyed.MapTypeKey => Event;

    public override string ToString() =>
        Event.DeclaredType.ToString();
}
