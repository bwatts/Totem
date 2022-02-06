using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Totem.Core;

namespace Totem.Queues;

public class QueueCommandInfo : CommandInfo
{
    public const string DefaultQueueName = "default";

    static readonly MessageInfoCache<QueueCommandInfo> _cache = new();

    QueueCommandInfo(Type declaredType, Text queueName) : base(declaredType) =>
        QueueName = queueName;

    public Text QueueName { get; }

    public static bool TryFrom(Type type, [NotNullWhen(true)] out QueueCommandInfo? info)
    {
        if(_cache.TryGetValue(type, out info))
        {
            return true;
        }

        if(type is not null && type.IsConcreteClass() && typeof(IQueueCommand).IsAssignableFrom(type))
        {
            var queueName = type.GetCustomAttribute<QueueNameAttribute>()?.Name;

            if(string.IsNullOrWhiteSpace(queueName))
            {
                queueName = DefaultQueueName;
            }

            info = new QueueCommandInfo(type, queueName.ToText());

            _cache.Add(info);

            return true;
        }

        info = null;
        return false;
    }

    public static QueueCommandInfo From(Type type)
    {
        if(!TryFrom(type, out var info))
            throw new ArgumentException($"Expected queue command {type} to be a public, non-abstract, non-or-closed-generic class implementing {typeof(IQueueCommand)}", nameof(type));

        return info;
    }
}
