namespace Totem.Map.Summary;

public class ObserverWhenSummary
{
    internal ObserverWhenSummary(string href, string name, ParameterSummary parameter, bool hasContext)
    {
        Href = href;
        Name = name;
        Parameter = parameter;
        HasContext = hasContext;
    }

    public string Href { get; }
    public string Name { get; }
    public ParameterSummary Parameter { get; }
    public bool HasContext { get; }
}
