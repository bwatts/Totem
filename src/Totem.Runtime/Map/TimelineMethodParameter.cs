using System;
using System.Reflection;

namespace Totem.Map;

public abstract class TimelineMethodParameter : IMapTypeKeyed
{
    internal TimelineMethodParameter(ParameterInfo info, MessageType message)
    {
        Info = info;
        Message = message;
    }

    public ParameterInfo Info { get; }
    public MessageType Message { get; }

    Type ITypeKeyed.TypeKey => Message.DeclaredType;
    MapType IMapTypeKeyed.MapTypeKey => Message;

    public override string ToString() =>
        Info.ToString();
}
