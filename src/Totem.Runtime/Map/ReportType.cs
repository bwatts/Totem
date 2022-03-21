using Totem.Reports;

namespace Totem.Map;

public class ReportType : ObserverType
{
    internal ReportType(Type declaredType, ObserverConstructor constructor) : base(declaredType, constructor)
    { }

    public TypeKeyedCollection<QueryType> Queries { get; } = new();

    internal new IReport Create(Id id) =>
        (IReport) base.Create(id);
}
