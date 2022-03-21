using System.Security.Claims;
using Totem.Core;
using Totem.Events;
using Totem.Map;
using Totem.Queues;
using Totem.Workflows;

namespace Totem;

public abstract class WorkflowTests<TWorkflow> : TimelineTests<TWorkflow>
    where TWorkflow : IWorkflow, new()
{
    readonly Queue<IQueueCommandEnvelope> _newCommands = new();

    internal override ITimelineTestContext<TWorkflow> TestContext { get; } = new TimelineTestContext<TWorkflow>();
    WorkflowType WorkflowType => (WorkflowType) TestContext.TimelineType;
    IWorkflow Workflow => TestContext.Timeline;

    protected void ExpectRoutesTo(Id expectedId, IEvent e)
    {
        if(expectedId is null)
            throw new ArgumentNullException(nameof(expectedId));

        if(e is null)
            throw new ArgumentNullException(nameof(e));

        var observation = GetObservation(e);

        if(observation.Route.ReturnsMany)
            throw new ExpectException($"Expected {TimelineMethod.Route}({e}) to return a single ID. Use a plural overload of {nameof(ExpectRoutesTo)} instead.");

        var routeId = GetObservation(e).Route.Call(e).Select(key => key.Id).Single();

        if(routeId != expectedId)
            throw new ExpectException($"Expected {TimelineMethod.Route}({e}) to return {expectedId} but received {routeId}");
    }

    protected void ExpectRoutesTo(Id expectedId0, Id expectedId1, IEvent e)
    {
        if(expectedId0 is null)
            throw new ArgumentNullException(nameof(expectedId0));

        if(expectedId1 is null)
            throw new ArgumentNullException(nameof(expectedId1));

        if(e is null)
            throw new ArgumentNullException(nameof(e));

        ExpectRoutesTo(new[] { expectedId0, expectedId1 }, e);
    }

    protected void ExpectRoutesTo(Id expectedId0, Id expectedId1, Id expectedId2, IEvent e)
    {
        if(expectedId0 is null)
            throw new ArgumentNullException(nameof(expectedId0));

        if(expectedId1 is null)
            throw new ArgumentNullException(nameof(expectedId1));

        if(expectedId2 is null)
            throw new ArgumentNullException(nameof(expectedId2));

        ExpectRoutesTo(new[] { expectedId0, expectedId1, expectedId2 }, e);
    }

    protected void ExpectRoutesTo(Id expectedId0, Id expectedId1, Id expectedId2, Id expectedId3, IEvent e)
    {
        if(expectedId0 is null)
            throw new ArgumentNullException(nameof(expectedId0));

        if(expectedId1 is null)
            throw new ArgumentNullException(nameof(expectedId1));

        if(expectedId2 is null)
            throw new ArgumentNullException(nameof(expectedId2));

        if(expectedId3 is null)
            throw new ArgumentNullException(nameof(expectedId3));

        ExpectRoutesTo(new[] { expectedId0, expectedId1, expectedId2, expectedId3 }, e);
    }

    protected void ExpectRoutesTo(IReadOnlyList<Id> expectedIds, IEvent e)
    {
        if(expectedIds is null)
            throw new ArgumentNullException(nameof(expectedIds));

        if(e is null)
            throw new ArgumentNullException(nameof(e));

        var observation = GetObservation(e);

        if(!observation.Route.ReturnsMany)
            throw new ExpectException($"Expected {TimelineMethod.Route}({e.GetType()}) to return many IDs. Use the singular overload of {nameof(ExpectRoutesTo)} instead.");

        var routeIds = observation.Route.Call(e).Select(key => key.Id).ToList();

        if(routeIds.Count != expectedIds.Count || expectedIds.Except(routeIds).Any())
            throw new ExpectException($"Expected route IDs [{string.Join(", ", expectedIds)}] but received [{string.Join(", ", routeIds)}]");
    }

    protected IEventContext<IEvent> CallWhen(Id routeId, IEvent e, Id? eventId = null, Id? correlationId = null, ClaimsPrincipal? principal = null, DateTimeOffset? whenOccurred = null)
    {
        if(routeId is null)
            throw new ArgumentNullException(nameof(routeId));

        if(e is null)
            throw new ArgumentNullException(nameof(e));

        OnCallingWhen();

        var observation = GetObservation(e);

        if(!observation.Route.ReturnsMany)
            throw new ExpectException($"Expected {TimelineMethod.Route}({e.GetType()}) to return many IDs. Use the {nameof(CallWhen)}({typeof(IEvent)}) overload instead.");

        var routeIds = observation.Route.Call(e).Select(key => key.Id).ToList();

        if(routeIds.Count != 1 || routeIds[0] != routeId)
            throw new ExpectException($"Expected single route {routeId} but received [{string.Join(", ", routeIds)}]");

        if(Workflow.Id is not null && routeId != Workflow.Id)
            throw new ExpectException($"Expected event to route to the same workflow {Workflow.Id} but received {routeId}");

        Workflow.Id = routeId;

        if(observation.When is null)
            throw new ExpectException($"Expected a {TimelineMethod.When} method for event {e.GetType()}");

        var context = CreateContext(e, eventId, correlationId, principal, whenOccurred);

        observation.When.Call(Workflow, context);

        foreach(var newCommand in Workflow.TakeNewCommands())
        {
            _newCommands.Enqueue(newCommand);
        }

        return context;
    }

    protected IEventContext<IEvent> CallWhen(IEvent e, Id? eventId = null, Id? correlationId = null, ClaimsPrincipal? principal = null, DateTimeOffset? whenOccurred = null)
    {
        if(e is null)
            throw new ArgumentNullException(nameof(e));

        OnCallingWhen();

        var observation = GetObservation(e);

        if(observation.Route.ReturnsMany)
            throw new ExpectException($"Expected {TimelineMethod.Route}({e.GetType()}) to return a single ID. Use the {nameof(CallWhen)}({typeof(Id)}, {typeof(IEvent)}) overload instead.");

        var routeId = GetObservation(e).Route.Call(e).Select(key => key.Id).Single();

        if(Workflow.Id is not null && routeId != Workflow.Id)
            throw new ExpectException($"Expected event to route to the same workflow {Workflow.Id} but found {routeId}");

        Workflow.Id = routeId;

        if(observation.When is null)
            throw new ExpectException($"Expected a {TimelineMethod.When} method for event {e.GetType()}");

        var context = CreateContext(e, eventId, correlationId, principal, whenOccurred);

        observation.When.Call(Workflow, context);

        foreach(var newCommand in Workflow.TakeNewCommands())
        {
            _newCommands.Enqueue(newCommand);
        }

        return context;
    }

    protected T ExpectCommand<T>() where T : IQueueCommand
    {
        OnExpectation();

        if(!_newCommands.TryDequeue(out var newCommand))
            throw new ExpectException($"Expected command of type {typeof(T)}");

        if(newCommand.Message is not T typedCommand)
            throw new ExpectException($"Expected command of type {typeof(T)} but found {newCommand.MessageKey.DeclaredType}");

        return typedCommand;
    }

    Observation GetObservation(IEvent e)
    {
        if(!WorkflowType.Observations.TryGet(e.GetType(), out var observation))
            throw new ExpectException($"Expected workflow {typeof(TWorkflow)} to observe event {e.GetType()}");

        return observation;
    }

    IEventContext<IEvent> CreateContext(IEvent e, Id? eventId, Id? correlationId, ClaimsPrincipal? principal, DateTimeOffset? whenOccurred)
    {
        var contextFactory = new EventContextFactory(TestContext.RuntimeMap);
        var pipelineId = Id.NewId();
        var eventType = e.GetType();
        var envelope = new EventEnvelope(
            new ItemKey(eventType, eventId ?? Id.NewId()),
            e,
            EventInfo.From(eventType),
            correlationId ?? Id.NewId(),
            principal ?? new ClaimsPrincipal(),
            whenOccurred ?? DateTimeOffset.Now);

        return contextFactory.Create(pipelineId, envelope);
    }
}
