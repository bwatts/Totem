using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Totem.Core;

namespace Totem.Local
{
    public class LocalCommandInfo : LocalMessageInfo
    {
        static readonly ConcurrentDictionary<Type, LocalCommandInfo> _infoByType = new();

        LocalCommandInfo(Type messageType) : base(messageType)
        { }

        public static bool TryFrom(Type type, [MaybeNullWhen(false)] out LocalCommandInfo info)
        {
            if(_infoByType.TryGetValue(type, out info))
            {
                return true;
            }

            if(type != null && type.IsConcreteClass() && typeof(ILocalCommand).IsAssignableFrom(type))
            {
                info = new LocalCommandInfo(type);

                _infoByType[type] = info;

                return true;
            }

            info = null;
            return false;
        }

        public static bool TryFrom(ILocalCommand command, [MaybeNullWhen(false)] out LocalCommandInfo info)
        {
            if(command == null)
                throw new ArgumentNullException(nameof(command));

            return TryFrom(command.GetType(), out info);
        }

        public static LocalCommandInfo From(Type type)
        {
            if(!TryFrom(type, out var info))
                throw new ArgumentException($"Expected command {type} to be a public, non-abstract, non-or-closed-generic class implementing {typeof(ILocalCommand)}", nameof(type));

            return info;
        }

        public static LocalCommandInfo From(ILocalCommand command)
        {
            if(command == null)
                throw new ArgumentNullException(nameof(command));

            return From(command.GetType());
        }
    }
}