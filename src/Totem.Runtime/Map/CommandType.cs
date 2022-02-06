using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Totem.Core;
using Totem.Topics;

namespace Totem.Map;

public class CommandType : MessageType
{
    internal CommandType(Type declaredType) : base(declaredType)
    { }

    public TypeKeyedCollection<TopicCommandContext> Contexts { get; } = new();
    public TopicRouteMethod? AnyContextRoute { get; internal set; }
    public TopicWhenMethod? AnyContextWhen { get; internal set; }

    internal bool TryCallRoute(ICommandContext<ICommandMessage> context, [NotNullWhen(true)] out ItemKey? topicKey)
    {
        if(Contexts.TryGet(context.InterfaceType, out var mapContext) && mapContext.Route is not null)
        {
            topicKey = mapContext.Route.Call(context);
            return true;
        }

        if(AnyContextRoute is not null)
        {
            topicKey = AnyContextRoute.Call(context);
            return true;
        }

        topicKey = null;
        return false;
    }

    internal async Task<bool> TryCallWhenAsync(ICommandContext<ICommandMessage> context, ITopic topic, CancellationToken cancellationToken)
    {
        if(Contexts.TryGet(context.InterfaceType, out var mapContext) && mapContext.When is not null)
        {
            await mapContext.When.CallAsync(context, topic, cancellationToken);
            return true;
        }

        if(AnyContextWhen is not null)
        {
            await AnyContextWhen.CallAsync(context, topic, cancellationToken);
            return true;
        }

        return false;
    }
}
