using System;
using System.Diagnostics.CodeAnalysis;

namespace Totem.Core;

public class EventInfo : MessageInfo
{
    static readonly MessageInfoCache<EventInfo> _cache = new();

    EventInfo(Type declaredType) : base(declaredType)
    { }

    public static bool TryFrom(Type type, [NotNullWhen(true)] out EventInfo? info)
    {
        if(_cache.TryGetValue(type, out info))
        {
            return true;
        }

        info = null;

        if(type is not null && type.IsConcreteClass() && typeof(IEvent).IsAssignableFrom(type))
        {
            info = new EventInfo(type);

            _cache.Add(info);

            return true;
        }

        return false;
    }

    public static EventInfo From(Type type)
    {
        if(!TryFrom(type, out var info))
            throw new ArgumentException($"Expected event {type} to be a public, non-abstract, non-or-closed-generic class implementing {typeof(IEvent)}", nameof(type));

        return info;
    }
}
