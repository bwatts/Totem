using System.Reflection;

namespace Totem.Map;

public abstract class TimelineMethod : IMapTypeKeyed
{
    public const string Route = nameof(Route);
    public const string Given = nameof(Given);
    public const string When = nameof(When);

    internal TimelineMethod(MethodInfo info, TimelineMethodParameter parameter)
    {
        Info = info;
        Parameter = parameter;
    }

    public MethodInfo Info { get; }
    public TimelineMethodParameter Parameter { get; }

    Type ITypeKeyed.TypeKey => Parameter.Message.DeclaredType;
    MapType IMapTypeKeyed.MapTypeKey => Parameter.Message;

    public override string ToString() =>
        Info.ToString() ?? $"{Info.DeclaringType}.{Info.Name}";
}
