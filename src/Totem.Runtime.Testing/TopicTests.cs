using Totem.Core;
using Totem.Map;
using Totem.Topics;

namespace Totem;

public abstract class TopicTests<TTopic> : TimelineTests<TTopic>, ITopicTests
    where TTopic : ITopic, new()
{
    readonly Queue<IEvent> _newEvents = new();
    TopicHttpContext? _httpContext;
    TopicLocalContext? _localContext;
    TopicQueueContext? _queueContext;

    internal override ITimelineTestContext<TTopic> TestContext { get; } = new TimelineTestContext<TTopic>();
    TopicType TopicType => (TopicType) TestContext.TimelineType;
    ITopic Topic => TestContext.Timeline;

    protected TopicHttpContext HttpContext => _httpContext ??= new(this);
    protected TopicLocalContext LocalContext => _localContext ??= new(this);
    protected TopicQueueContext QueueContext => _queueContext ??= new(this);

    RuntimeMap ITopicTests.RuntimeMap => TestContext.RuntimeMap;
    ITopic ITopicTests.Topic => TestContext.Timeline;

    void ITopicTests.OnCallingWhen() =>
        OnCallingWhen();

    void ITopicTests.OnWhenCalled()
    {
        foreach(var newEvent in Topic.TakeNewEvents())
        {
            TopicType.CallGivenIfDefined(Topic, newEvent);

            _newEvents.Enqueue(newEvent);
        }
    }

    void ITopicTests.OnExpectingCommand() =>
        OnExpectation();

    protected void ExpectRoutesTo(Id expectedId, ICommandMessage command)
    {
        if(expectedId is null)
            throw new ArgumentNullException(nameof(expectedId));

        if(command is null)
            throw new ArgumentNullException(nameof(command));

        var routeKey = GetCommandType(command).Route.Call(command);

        if(routeKey.Id != expectedId)
            throw new ExpectException($"Expected {TimelineMethod.Route}({command.GetType()}) to return {expectedId} but found {routeKey.Id}");
    }

    protected T ExpectEvent<T>() where T : IEvent
    {
        OnExpectation();

        if(!_newEvents.TryDequeue(out var newEvent))
            throw new ExpectException($"Expected event of type {typeof(T)}");

        if(newEvent is not T typedEvent)
            throw new ExpectException($"Expected event of type {typeof(T)} but found {newEvent.GetType()}");

        return typedEvent;
    }

    CommandType GetCommandType(ICommandMessage command)
    {
        if(!TopicType.Commands.TryGet(command.GetType(), out var commandType))
            throw new ExpectException($"Expected topic {typeof(TTopic)} to handle command {command.GetType()}");

        return commandType;
    }
}
