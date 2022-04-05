using System.Security.Claims;
using Totem.Map;
using Totem.Topics;

namespace Totem.Core;

public abstract class TopicContext<TCommand, TContext>
    where TCommand : ICommandMessage
    where TContext : ICommandContext<ICommandMessage>
{
    internal TopicContext(ITopicTests tests) =>
        Tests = tests;

    internal ITopicTests Tests { get; }
    TopicType TopicType => Tests.TopicType;
    ITopic Topic => Tests.Topic;

    public TContext CallWhen(TCommand command, Id? commandId = null, Id? correlationId = null, ClaimsPrincipal? principal = null)
    {
        if(command is null)
            throw new ArgumentNullException(nameof(command));

        Tests.OnCallingWhen();

        var (context, when) = InitWhenCall(command, commandId, correlationId, principal);

        if(when.IsAsync)
            throw new ExpectException($"Expected {TimelineMethod.When}({command.GetType()}) to be void. Use {nameof(CallWhenAsync)} instead.");

        when.CallAsync(context, Topic, CancellationToken.None).Wait();

        Tests.OnWhenCalled();

        return context;
    }

    public async Task<TContext> CallWhenAsync(
        TCommand command,
        Id? commandId = null,
        Id? correlationId = null,
        ClaimsPrincipal? principal = null,
        CancellationToken cancellationToken = default)
    {
        if(command is null)
            throw new ArgumentNullException(nameof(command));

        Tests.OnCallingWhen();

        var (context, when) = InitWhenCall(command, commandId, correlationId, principal);

        if(!when.IsAsync)
            throw new ExpectException($"Expected {TimelineMethod.When}({command.GetType()}) to be async. Use {nameof(CallWhen)} instead.");

        await when.CallAsync(context, Topic, cancellationToken);

        Tests.OnWhenCalled();

        return context;
    }

    (TContext, TopicWhenMethod) InitWhenCall(TCommand command, Id? commandId, Id? correlationId, ClaimsPrincipal? principal)
    {
        var context = CreateContext(
            command,
            commandId ?? Id.NewId(),
            correlationId ?? Id.NewId(),
            principal ?? new ClaimsPrincipal());

        if(!Tests.RuntimeMap.Commands.TryGet(command.GetType(), out var commandType))
            throw new ExpectException($"Expected a command type known to the map: {command.GetType()}");

        if(TopicType.SingleInstanceId is not null)
        {
            Topic.Id = TopicType.SingleInstanceId;
        }
        else
        {
            var routeKey = commandType.CallRoute(context);

            if(Topic.Id is not null && Topic.Id != routeKey.Id)
                throw new ExpectException($"Expected command to route to the same topic {Topic.Id} but received {routeKey.Id}");

            Topic.Id = routeKey.Id;
        }

        if(!commandType.TryGetWhen(context, out var when))
            throw new ExpectException($"Expected a {TimelineMethod.When} method for command {command.GetType()}");

        return (context, when);
    }

    protected abstract TContext CreateContext(TCommand command, Id commandId, Id correlationId, ClaimsPrincipal principal);
}
