namespace Totem.Map;

public abstract class TimelineType : MapType
{
    static readonly Id _singleInstanceNamespace = (Id) "adae821d-c9bd-4c56-b3c3-aab90324a2e1";

    internal TimelineType(Type declaredType) : base(declaredType)
    { }

    internal static Id GetSingleInstanceId(Type declaredType) =>
        _singleInstanceNamespace.DeriveId(declaredType.FullName ?? "");
}
