namespace Totem.Map.Summary;

public class ObserverRouteSummary
{
    internal ObserverRouteSummary(string href, string name, ParameterSummary parameter, bool returnsMany)
    {
        Href = href;
        Name = name;
        Parameter = parameter;
        ReturnsMany = returnsMany;
    }

    public string Href { get; }
    public string Name { get; }
    public ParameterSummary Parameter { get; }
    public bool ReturnsMany { get; }
}
