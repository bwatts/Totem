namespace Totem.Map.Summary;

public class ObservationSummary
{
    internal ObservationSummary(string href, Id eventTypeId, ObserverRouteSummary route, GivenSummary? given, ObserverWhenSummary? when)
    {
        Href = href;
        EventTypeId = eventTypeId;
        Route = route;
        Given = given;
        When = when;
    }

    public string Href { get; }
    public Id EventTypeId { get; }
    public ObserverRouteSummary Route { get; }
    public GivenSummary? Given { get; }
    public ObserverWhenSummary? When { get; }
}
