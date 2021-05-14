using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Totem
{
    public class QueueCommandInfo
    {
        public const string DefaultQueueName = "default";
        static readonly ConcurrentDictionary<Type, QueueCommandInfo> _infosByType = new();

        QueueCommandInfo(Type commandType, Text queueName)
        {
            CommandType = commandType;
            QueueName = queueName;
        }

        public Type CommandType { get; }
        public Text QueueName { get; }

        public static bool IsQueueCommand(Type type) =>
            TryFrom(type, out var _);

        public static bool TryFrom(Type type, [MaybeNullWhen(false)] out QueueCommandInfo info)
        {
            if(_infosByType.TryGetValue(type, out info))
            {
                return true;
            }

            info = null;

            if(type == null || !type.IsPublic || !type.IsClass || type.IsAbstract || type.ContainsGenericParameters)
            {
                return false;
            }

            if(typeof(IQueueCommand).IsAssignableFrom(type))
            {
                var queueName = type.GetCustomAttribute<QueueNameAttribute>()?.Name ?? DefaultQueueName;

                info = new QueueCommandInfo(type, queueName.ToText());

                _infosByType[type] = info;

                return true;
            }

            info = null;
            return false;
        }

        public static bool TryFrom(IQueueCommand command, [MaybeNullWhen(false)] out QueueCommandInfo info)
        {
            if(command == null)
                throw new ArgumentNullException(nameof(command));

            return TryFrom(command.GetType(), out info);
        }

        public static QueueCommandInfo From(Type type)
        {
            if(!TryFrom(type, out var info))
                throw new ArgumentException($"Expected queue command type to be a public, non-abstract, non-or-closed-generic class implementing {typeof(IQueueCommand)}: {type}", nameof(type));

            return info;
        }

        public static QueueCommandInfo From(IQueueCommand command)
        {
            if(command == null)
                throw new ArgumentNullException(nameof(command));

            return From(command.GetType());
        }
    }
}