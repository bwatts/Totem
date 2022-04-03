namespace Totem.Map.Summary;

public class ObservationSummary
{
    internal ObservationSummary(Id eventTypeId, ObserverRouteSummary route, GivenSummary? given, ObserverWhenSummary? when)
    {
        EventTypeId = eventTypeId;
        Route = route;
        Given = given;
        When = when;
    }

    public Id EventTypeId { get; }
    public ObserverRouteSummary Route { get; }
    public GivenSummary? Given { get; }
    public ObserverWhenSummary? When { get; }
}
