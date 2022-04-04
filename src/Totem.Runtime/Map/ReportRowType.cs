namespace Totem.Map;

public class ReportRowType : MapType
{
    internal ReportRowType(Type declaredType, IReadOnlyList<ReportRowProperty> properties) : base(declaredType) =>
        Properties = properties;

    public IReadOnlyList<ReportRowProperty> Properties { get; }
}
