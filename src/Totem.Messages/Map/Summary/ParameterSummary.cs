namespace Totem.Map.Summary;

public class ParameterSummary
{
    internal ParameterSummary(string name, Id parameterTypeId, Id messageTypeId)
    {
        Name = name;
        ParameterTypeId = parameterTypeId;
        MessageTypeId = messageTypeId;
    }

    public string Name { get; }
    public Id ParameterTypeId { get; }
    public Id MessageTypeId { get; }
}
