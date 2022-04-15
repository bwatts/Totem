using System.Collections;
using Totem.Core;
using Totem.Http;
using Totem.Local;
using Totem.Map.Builder;

namespace Totem.Map;

public class RuntimeMap : IReadOnlyCollection<MapType>
{
    readonly List<IMapCheck> _checks = new();
    readonly List<IMapCheck> _failedChecks = new();

    public RuntimeMap(IEnumerable<Type> types)
    {
        if(types is null)
            throw new ArgumentNullException(nameof(types));

        new MapBuilder(this, types).Build();
    }

    public RuntimeMap(params Type[] types) : this(types as IEnumerable<Type>)
    { }

    public TypeKeyedCollection<CommandType> Commands = new();
    public TypeKeyedCollection<EventType> Events = new();
    public TypeKeyedCollection<EventHandlerType> EventHandlers = new();
    public TypeKeyedCollection<QueryType> Queries = new();
    public TypeKeyedCollection<QueryHandlerType> QueryHandlers = new();
    public TypeKeyedCollection<ReportType> Reports = new();
    public TypeKeyedCollection<ReportRowType> ReportRows = new();
    public TypeKeyedCollection<TopicType> Topics = new();
    public TypeKeyedCollection<WorkflowType> Workflows = new();
    public IReadOnlyList<IMapCheck> Checks => _checks;
    public IEnumerable<IMapCheck> FailedChecks => _failedChecks;

    public int Count =>
        Commands.Count
        + Events.Count
        + EventHandlers.Count
        + Queries.Count
        + QueryHandlers.Count
        + Reports.Count
        + ReportRows.Count
        + Topics.Count
        + Workflows.Count;

    public IEnumerator<MapType> GetEnumerator() =>
        Commands
        .Concat<MapType>(Events)
        .Concat(EventHandlers)
        .Concat(Queries)
        .Concat(QueryHandlers)
        .Concat(Reports)
        .Concat(ReportRows)
        .Concat(Topics)
        .Concat(Workflows)
        .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    internal void AddCheck(IMapCheck check)
    {
        _checks.Add(check);

        if(!check.HasOutput)
        {
            _failedChecks.Add(check);
        }
    }

    internal CommandType GetOrAddCommand(Type declaredType)
    {
        if(!Commands.TryGet(declaredType, out var command))
        {
            command = new CommandType(declaredType);

            Commands.Add(command);
        }

        return command;
    }

    internal EventType GetOrAddEvent(Type declaredType)
    {
        if(!Events.TryGet(declaredType, out var e))
        {
            e = new EventType(EventInfo.From(declaredType));

            Events.Add(e);
        }

        return e;
    }

    internal QueryType GetOrAddQuery(Type declaredType)
    {
        if(!Queries.TryGet(declaredType, out var query))
        {
            query = new QueryType(declaredType);

            Queries.Add(query);
        }

        return query;
    }

    internal IEnumerable<ItemKey> CallReportRoutes(IEventContext<IEvent> context)
    {
        if(Events.TryGet(context.EventType.DeclaredType, out var e))
        {
            foreach(var report in e.Reports)
            {
                foreach(var reportKey in report.CallRoute(context))
                {
                    yield return reportKey;
                }
            }
        }
    }

    internal IEnumerable<ItemKey> CallWorkflowRoutes(IEventContext<IEvent> context)
    {
        if(Events.TryGet(context.EventType.DeclaredType, out var e))
        {
            foreach(var workflow in e.Workflows)
            {
                foreach(var workflowKey in workflow.CallRoute(context))
                {
                    yield return workflowKey;
                }
            }
        }
    }
}
