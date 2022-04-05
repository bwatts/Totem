using Totem.Core;

namespace Totem.Map;

public abstract class TimelineType : MapType
{
    static readonly Id _singleInstanceNamespace = (Id) "adae821d-c9bd-4c56-b3c3-aab90324a2e1";

    internal TimelineType(Type declaredType) : base(declaredType)
    { }

    public Id? SingleInstanceId { get; private set; }

    internal void SetSingleInstanceId() =>
        SingleInstanceId = _singleInstanceNamespace.DeriveId(DeclaredType.FullName ?? "");

    internal ItemKey CallSingleInstanceRoute()
    {
        if(SingleInstanceId is null)
            throw new InvalidOperationException($"Timeline {this} is not single-instance");

        return new(DeclaredType, SingleInstanceId);
    }
}
