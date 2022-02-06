namespace Totem.Map.Summary;

public class ObserverRouteSummary
{
    internal ObserverRouteSummary(string href, string name, ParameterSummary parameter, bool hasContext, bool returnsMany)
    {
        Href = href;
        Name = name;
        Parameter = parameter;
        HasContext = hasContext;
        ReturnsMany = returnsMany;
    }

    public string Href { get; }
    public string Name { get; }
    public ParameterSummary Parameter { get; }
    public bool HasContext { get; }
    public bool ReturnsMany { get; }
}
