using Totem.Core;
using Totem.Http;
using Totem.Local;
using Totem.Queues;

namespace Totem.Map.Summary;

internal class MapSummaryBuilder
{
    static readonly Id _typeIdNamespace = (Id) "98970055-1b9e-4537-a2be-b004aa6d2cb9";

    readonly Dictionary<Type, Id> _typeIdsByType = new();
    readonly List<TypeSummary> _types = new();
    readonly RuntimeMap _map;

    internal MapSummaryBuilder(RuntimeMap map) =>
        _map = map;

    internal MapSummary Build() =>
        new(_types,
            BuildCommands().ToList(),
            BuildEvents().ToList(),
            BuildEventHandlers().ToList(),
            BuildQueries().ToList(),
            BuildQueryHandlers().ToList(),
            BuildReports().ToList(),
            BuildReportRows().ToList(),
            BuildTopics().ToList(),
            BuildWorkflows().ToList());

    IEnumerable<CommandSummary> BuildCommands()
    {
        foreach(var command in _map.Commands)
        {
            var commandTypeId = GetTypeId(command);
            var httpContext = null as HttpCommandContextSummary;
            var localContext = null as LocalCommandContextSummary;
            var queueContext = null as QueueCommandContextSummary;

            if(command.HttpContext is not null)
            {
                var httpInfo = (HttpCommandInfo) command.HttpContext.Info;

                httpContext = new HttpCommandContextSummary(
                    GetTypeId(command.HttpContext.InterfaceType),
                    BuildHttpRequest(httpInfo.Request),
                    TryBuildTopicWhen(command.HttpContext.When));
            }

            if(command.LocalContext is not null)
            {
                localContext = new LocalCommandContextSummary(
                    GetTypeId(command.LocalContext.InterfaceType),
                    TryBuildTopicWhen(command.LocalContext.When));
            }

            if(command.QueueContext is not null)
            {
                var queueInfo = (QueueCommandInfo) command.QueueContext.Info;

                queueContext = new QueueCommandContextSummary(
                    GetTypeId(command.QueueContext.InterfaceType),
                    queueInfo.QueueName,
                    TryBuildTopicWhen(command.QueueContext.When));
            }

            yield return new CommandSummary(
                commandTypeId,
                BuildTopicRoute(command.Route),
                TryBuildTopicWhen(command.WhenWithoutContext),
                httpContext,
                localContext,
                queueContext);
        }
    }

    IEnumerable<EventSummary> BuildEvents() =>
        from e in _map.Events
        let typeId = GetTypeId(e)
        select new EventSummary(
            typeId,
            e.Reports.Select(GetTypeId).ToList(),
            e.Workflows.Select(GetTypeId).ToList(),
            e.Handlers.Select(GetTypeId).ToList());

    IEnumerable<EventHandlerSummary> BuildEventHandlers() =>
        from eventHandler in _map.EventHandlers
        select new EventHandlerSummary(
            GetTypeId(eventHandler),
            GetTypeId(eventHandler.ServiceType));

    IEnumerable<QuerySummary> BuildQueries()
    {
        foreach(var query in _map.Queries)
        {
            var queryTypeId = GetTypeId(query);

            var httpContext = null as HttpQueryContextSummary;
            var localContext = null as LocalQueryContextSummary;

            foreach(var context in query.Contexts)
            {
                if(context.Info is HttpQueryInfo httpInfo)
                {
                    httpContext = new HttpQueryContextSummary(
                        GetTypeId(context.ContextType),
                        BuildHttpRequest(httpInfo.Request),
                        BuildQueryResult(httpInfo.Result));
                }
                else if(context.Info is LocalQueryInfo localInfo)
                {
                    localContext = new LocalQueryContextSummary(
                        GetTypeId(localInfo.Result.DeclaredType),
                        BuildQueryResult(localInfo.Result));
                }
                else
                {
                    throw new NotSupportedException($"Query info type {context.Info.DeclaredType} is not supported");
                }
            }

            yield return new QuerySummary(queryTypeId, httpContext, localContext);
        }
    }

    IEnumerable<QueryHandlerSummary> BuildQueryHandlers() =>
        from queryHandler in _map.QueryHandlers
        let typeId = GetTypeId(queryHandler)
        let queryTypeId = GetTypeId(queryHandler.Query)
        select new QueryHandlerSummary(typeId, queryHandler.ServiceTypes.Select(GetTypeId).ToList());

    IEnumerable<ReportSummary> BuildReports() =>
        from report in _map.Reports
        let typeId = GetTypeId(report)
        let observations =
            from observation in report.Observations
            let eventTypeId = GetTypeId(observation.Event)
            select new ObservationSummary(
                eventTypeId,
                BuildObserverRoute(observation.Route),
                TryBuildGiven(observation.Given),
                TryBuildObserverWhen(observation.When))
        select new ReportSummary(typeId, observations.ToList(), report.Queries.Select(GetTypeId).ToList());

    IEnumerable<ReportRowSummary> BuildReportRows() =>
        from reportRow in _map.ReportRows
        let properties =
            from property in reportRow.Properties
            select new ReportRowPropertySummary(property.Name, GetTypeId(property.ValueType))
        select new ReportRowSummary(GetTypeId(reportRow), properties.ToList());

    IEnumerable<TopicSummary> BuildTopics() =>
        from topic in _map.Topics
        let typeId = GetTypeId(topic)
        select new TopicSummary(
            typeId,
            topic.Commands.Select(GetTypeId).ToList(),
            topic.Givens.Select(x => TryBuildGiven(x)!).ToList());

    IEnumerable<WorkflowSummary> BuildWorkflows() =>
        from workflow in _map.Workflows
        let typeId = GetTypeId(workflow)
        let observations =
            from observation in workflow.Observations
            let eventTypeId = GetTypeId(observation.Event)
            select new ObservationSummary(
                eventTypeId,
                BuildObserverRoute(observation.Route),
                TryBuildGiven(observation.Given),
                TryBuildObserverWhen(observation.When))
        select new WorkflowSummary(typeId, observations.ToList());

    GivenSummary? TryBuildGiven(GivenMethod? given) =>
        given is null ? null : new(BuildParameter(given.Parameter));

    ParameterSummary BuildParameter(TimelineMethodParameter parameter) =>
        new(parameter.Info.Name ?? "",
            GetTypeId(parameter.Info.ParameterType),
            GetTypeId(parameter.Message));

    TopicRouteSummary? BuildTopicRoute(TopicRouteMethod? route) =>
        route is null ? null : new(BuildParameter(route.Parameter));

    ObserverRouteSummary? BuildObserverRoute(ObserverRouteMethod? route) =>
        route is null ? null : new(BuildParameter(route.Parameter), route.ReturnsMany);

    TopicWhenSummary? TryBuildTopicWhen(TopicWhenMethod? when) =>
        when is null ? null : new(BuildParameter(when.Parameter), when.IsAsync, when.HasCancellationToken);

    ObserverWhenSummary? TryBuildObserverWhen(ObserverWhenMethod? when) =>
        when is null ? null : new(BuildParameter(when.Parameter), when.Parameter.HasContext);

    static HttpRequestSummary BuildHttpRequest(HttpRequestInfo info) =>
        new(info.Method, info.Route);

    QueryResultSummary BuildQueryResult(QueryResult result) =>
        new(GetTypeId(result.DeclaredType),
            result.IsReport,
            result.IsSingleRow,
            result.IsManyRows,
            result.RowType is null ? null : GetTypeId(result.RowType));

    Id GetTypeId(Type type)
    {
        if(!_typeIdsByType.TryGetValue(type, out var id))
        {
            var assemblyQualifiedName = type.AssemblyQualifiedName ?? "";

            id = _typeIdNamespace.DeriveId(assemblyQualifiedName);

            _typeIdsByType[type] = id;
            _types.Add(new(id, type.Namespace ?? "", type.Name, type.FullName ?? "", assemblyQualifiedName));
        }

        return id;
    }

    Id GetTypeId(MapType type) =>
        GetTypeId(type.DeclaredType);
}
