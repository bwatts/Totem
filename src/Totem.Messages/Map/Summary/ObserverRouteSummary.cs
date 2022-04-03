namespace Totem.Map.Summary;

public class ObserverRouteSummary
{
    internal ObserverRouteSummary(ParameterSummary parameter, bool returnsMany)
    {
        Parameter = parameter;
        ReturnsMany = returnsMany;
    }

    public ParameterSummary Parameter { get; }
    public bool ReturnsMany { get; }
}
