namespace Totem.Map.Summary;

public class TopicRouteSummary
{
    internal TopicRouteSummary(string href, string name, ParameterSummary parameter)
    {
        Href = href;
        Name = name;
        Parameter = parameter;
    }

    public string Href { get; }
    public string Name { get; }
    public ParameterSummary Parameter { get; }
}
