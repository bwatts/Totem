using Totem.Reports;

namespace Totem.Map;

public class ReportType : ObserverType
{
    internal ReportType(Type declaredType, ObserverConstructor constructor, ReportRowType row) : base(declaredType, constructor) =>
        Row = row;

    public ReportRowType Row { get; }
    public TypeKeyedCollection<QueryType> Queries { get; } = new();

    internal new IReport Create(Id id) =>
        (IReport) base.Create(id);
}
