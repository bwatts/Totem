namespace Totem.Map.Summary;

public class TopicRouteSummary
{
    internal TopicRouteSummary(ParameterSummary parameter) =>
        Parameter = parameter;

    public ParameterSummary Parameter { get; }
}
