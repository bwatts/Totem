namespace Totem.Map.Summary;

public class GivenSummary
{
    internal GivenSummary(string href, string name, ParameterSummary parameter)
    {
        Href = href;
        Name = name;
        Parameter = parameter;
    }

    public string Href { get; }
    public string Name { get; }
    public ParameterSummary Parameter { get; }
}
