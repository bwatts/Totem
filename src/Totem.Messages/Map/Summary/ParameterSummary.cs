namespace Totem.Map.Summary;

public class ParameterSummary
{
    internal ParameterSummary(string href, string name, Id parameterTypeId, Id messageTypeId)
    {
        Href = href;
        Name = name;
        ParameterTypeId = parameterTypeId;
        MessageTypeId = messageTypeId;
    }

    public string Href { get; }
    public string Name { get; }
    public Id ParameterTypeId { get; }
    public Id MessageTypeId { get; }
}
