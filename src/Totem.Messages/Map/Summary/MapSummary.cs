using System.Collections.Generic;

namespace Totem.Map.Summary;

public class MapSummary
{
    internal MapSummary(
        IReadOnlyList<TypeSummary> types,
        IReadOnlyList<CommandSummary> commands,
        IReadOnlyList<EventSummary> events,
        IReadOnlyList<EventHandlerSummary> eventHandlers,
        IReadOnlyList<QuerySummary> queries,
        IReadOnlyList<QueryHandlerSummary> queryHandlers,
        IReadOnlyList<ReportSummary> reports,
        IReadOnlyList<TopicSummary> topics,
        IReadOnlyList<WorkflowSummary> workflows)
    {
        Types = types;
        Commands = commands;
        Events = events;
        EventHandlers = eventHandlers;
        Queries = queries;
        QueryHandlers = queryHandlers;
        Reports = reports;
        Topics = topics;
        Workflows = workflows;
    }

    public IReadOnlyList<TypeSummary> Types { get; }
    public IReadOnlyList<CommandSummary> Commands { get; }
    public IReadOnlyList<EventSummary> Events { get; }
    public IReadOnlyList<EventHandlerSummary> EventHandlers { get; }
    public IReadOnlyList<QuerySummary> Queries { get; }
    public IReadOnlyList<QueryHandlerSummary> QueryHandlers { get; }
    public IReadOnlyList<ReportSummary> Reports { get; }
    public IReadOnlyList<TopicSummary> Topics { get; }
    public IReadOnlyList<WorkflowSummary> Workflows { get; }
}
