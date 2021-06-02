using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Totem.Core;

namespace Totem.Queues
{
    public class QueueCommandInfo : MessageInfo
    {
        public const string DefaultQueueName = "default";
        static readonly ConcurrentDictionary<Type, QueueCommandInfo> _infoByType = new();

        QueueCommandInfo(Type messageType, Text queueName) : base(messageType) =>
            QueueName = queueName ?? throw new ArgumentNullException(nameof(queueName));

        public Text QueueName { get; }

        public static bool TryFrom(Type type, [MaybeNullWhen(false)] out QueueCommandInfo info)
        {
            if(_infoByType.TryGetValue(type, out info))
            {
                return true;
            }

            if(type != null && type.IsConcreteClass() && typeof(IQueueCommand).IsAssignableFrom(type))
            {
                var queueName = type.GetCustomAttribute<QueueNameAttribute>()?.Name;

                if(string.IsNullOrWhiteSpace(queueName))
                {
                    queueName = DefaultQueueName;
                }

                info = new QueueCommandInfo(type, queueName.ToText());

                _infoByType[type] = info;

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
                throw new ArgumentException($"Expected queue command {type} to be a public, non-abstract, non-or-closed-generic class implementing {typeof(IQueueCommand)}", nameof(type));

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