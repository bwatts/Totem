using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Totem.Core
{
    public class EventInfo : MessageInfo
    {
        static readonly ConcurrentDictionary<Type, EventInfo> _infoByType = new();

        EventInfo(Type messageType) : base(messageType)
        { }

        public static bool TryFrom(Type type, [MaybeNullWhen(false)] out EventInfo info)
        {
            if(_infoByType.TryGetValue(type, out info))
            {
                return true;
            }

            info = null;

            if(type != null && type.IsConcreteClass() && typeof(IEvent).IsAssignableFrom(type))
            {
                info = new EventInfo(type);

                _infoByType[type] = info;

                return true;
            }

            return false;
        }

        public static bool TryFrom(IEvent e, [MaybeNullWhen(false)] out EventInfo info)
        {
            if(e == null)
                throw new ArgumentNullException(nameof(e));

            return TryFrom(e.GetType(), out info);
        }

        public static EventInfo From(Type type)
        {
            if(!TryFrom(type, out var info))
                throw new ArgumentException($"Expected event {type} to be a public, non-abstract, non-or-closed-generic class implementing {typeof(IEvent)}", nameof(type));

            return info;
        }

        public static EventInfo From(IEvent e)
        {
            if(e == null)
                throw new ArgumentNullException(nameof(e));

            return From(e.GetType());
        }
    }
}