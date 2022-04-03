namespace Totem.Map.Summary;

public class ObserverWhenSummary
{
    internal ObserverWhenSummary(ParameterSummary parameter, bool hasContext)
    {
        Parameter = parameter;
        HasContext = hasContext;
    }

    public ParameterSummary Parameter { get; }
    public bool HasContext { get; }
}
