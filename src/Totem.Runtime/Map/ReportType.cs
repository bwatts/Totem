namespace Totem.Map;

public class ReportType : ObserverType
{
    internal ReportType(Type declaredType) : base(declaredType)
    { }

    public TypeKeyedCollection<QueryType> Queries { get; } = new();
}
