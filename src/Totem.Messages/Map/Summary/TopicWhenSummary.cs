namespace Totem.Map.Summary;

public class TopicWhenSummary
{
    internal TopicWhenSummary(ParameterSummary parameter, bool isAsync, bool hasCancellationToken)
    {
        Parameter = parameter;
        IsAsync = isAsync;
        HasCancellationToken = hasCancellationToken;
    }

    public ParameterSummary Parameter { get; }
    public bool IsAsync { get; }
    public bool HasCancellationToken { get; }
}
