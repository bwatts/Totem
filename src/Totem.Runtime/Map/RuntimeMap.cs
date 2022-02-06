using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Totem.Core;
using Totem.Map.Builder;

namespace Totem.Map;

public class RuntimeMap : IReadOnlyCollection<MapType>
{
    readonly List<IMapCheck> _checks = new();

    public TypeKeyedCollection<CommandType> Commands = new();
    public TypeKeyedCollection<EventType> Events = new();
    public TypeKeyedCollection<EventHandlerType> EventHandlers = new();
    public TypeKeyedCollection<QueryType> Queries = new();
    public TypeKeyedCollection<QueryHandlerType> QueryHandlers = new();
    public TypeKeyedCollection<ReportType> Reports = new();
    public TypeKeyedCollection<TopicType> Topics = new();
    public TypeKeyedCollection<WorkflowType> Workflows = new();
    public IReadOnlyList<IMapCheck> Checks => _checks;

    public int Count =>
        Commands.Count
        + Events.Count
        + EventHandlers.Count
        + Queries.Count
        + QueryHandlers.Count
        + Reports.Count
        + Topics.Count
        + Workflows.Count;

    public IEnumerator<MapType> GetEnumerator() =>
        Commands
        .Concat<MapType>(Events)
        .Concat(EventHandlers)
        .Concat(Queries)
        .Concat(QueryHandlers)
        .Concat(Reports)
        .Concat(Topics)
        .Concat(Workflows)
        .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal void AddCheck(IMapCheck check) =>
        _checks.Add(check);

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
