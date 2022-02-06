namespace Totem.Map.Summary;

public class TopicWhenSummary
{
    internal TopicWhenSummary(string href, string name, ParameterSummary parameter, bool isAsync, bool hasCancellationToken)
    {
        Href = href;
        Name = name;
        Parameter = parameter;
        IsAsync = isAsync;
        HasCancellationToken = hasCancellationToken;
    }

    public string Href { get; }
    public string Name { get; }
    public ParameterSummary Parameter { get; }
    public bool IsAsync { get; }
    public bool HasCancellationToken { get; }
}
