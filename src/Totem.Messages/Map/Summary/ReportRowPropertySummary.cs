namespace Totem.Map.Summary;

public class ReportRowPropertySummary
{
    internal ReportRowPropertySummary(string name, Id valueTypeId)
    {
        Name = name;
        ValueTypeId = valueTypeId;
    }

    public string Name { get; }
    public Id ValueTypeId { get; }
}
