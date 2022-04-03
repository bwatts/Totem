namespace Totem.Map.Summary;

public class GivenSummary
{
    internal GivenSummary(ParameterSummary parameter) =>
        Parameter = parameter;

    public ParameterSummary Parameter { get; }
}
