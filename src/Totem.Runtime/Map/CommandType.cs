using Totem.Core;
using Totem.Topics;

namespace Totem.Map;

public class CommandType : MessageType
{
    internal CommandType(Type declaredType) : base(declaredType)
    { }

    public TopicType Topic { get; internal set; } = null!;
    public TopicRouteMethod? Route { get; internal set; }
    public TopicWhenMethod? WhenWithoutContext { get; internal set; }
    public TopicWhenContext? HttpContext { get; internal set; }
    public TopicWhenContext? LocalContext { get; internal set; }
    public TopicWhenContext? QueueContext { get; internal set; }

    internal ItemKey CallRoute(ICommandContext<ICommandMessage> context) =>
        Route?.Call(context.Command) ?? Topic.CallSingleInstanceRoute();

    internal bool TryGetWhen(ICommandContext<ICommandMessage> context, [NotNullWhen(true)] out TopicWhenMethod? when)
    {
        if(HttpContext is not null && HttpContext.TryGetWhen(context, out when))
        {
            return true;
        }

        if(LocalContext is not null && LocalContext.TryGetWhen(context, out when))
        {
            return true;
        }

        if(QueueContext is not null && QueueContext.TryGetWhen(context, out when))
        {
            return true;
        }

        if(WhenWithoutContext is not null)
        {
            when = WhenWithoutContext;
            return true;
        }

        when = null;
        return false;
    }

    internal async Task<bool> TryCallWhenAsync(ICommandContext<ICommandMessage> context, ITopic topic, CancellationToken cancellationToken)
    {
        if(TryGetWhen(context, out var when))
        {
            await when.CallAsync(context, topic, cancellationToken);
            return true;
        }

        return false;
    }
}
