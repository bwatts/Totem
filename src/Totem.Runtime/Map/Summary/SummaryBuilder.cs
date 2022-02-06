using System;
using System.Collections.Generic;
using System.Linq;
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
            BuildTopics().ToList(),
            BuildWorkflows().ToList());

    IEnumerable<CommandSummary> BuildCommands()
    {
        foreach(var command in _map.Commands)
        {
            var commandTypeId = GetTypeId(command);
            var commandHref = $"/commands/{commandTypeId}";

            var httpContext = null as HttpCommandContextSummary;
            var localContext = null as LocalCommandContextSummary;
            var queueContext = null as QueueCommandContextSummary;

            foreach(var context in command.Contexts)
            {
                if(context.Info is HttpCommandInfo httpInfo)
                {
                    var contextHref = $"{commandHref}/contexts/http";

                    httpContext = new HttpCommandContextSummary(
                        contextHref,
                        GetTypeId(context.ContextType),
                        BuildHttpRequest(contextHref, httpInfo.Request),
                        BuildRoute(contextHref, context.Route),
                        BuildWhen(contextHref, context.When));
                }
                else if(context.Info is LocalCommandInfo localInfo)
                {
                    var contextHref = $"{commandHref}/contexts/local";

                    localContext = new LocalCommandContextSummary(
                        contextHref,
                        GetTypeId(context.ContextType),
                        BuildRoute(contextHref, context.Route),
                        BuildWhen(contextHref, context.When));
                }
                else if(context.Info is QueueCommandInfo queueInfo)
                {
                    var contextHref = $"{commandHref}/contexts/queue";

                    queueContext = new QueueCommandContextSummary(
                        contextHref,
                        GetTypeId(context.ContextType),
                        queueInfo.QueueName,
                        BuildRoute(contextHref, context.Route),
                        BuildWhen(contextHref, context.When));
                }
                else
                {
                    throw new NotSupportedException($"Command info type {context.Info.DeclaredType} is not supported");
                }
            }

            var anyContextHref = $"{commandHref}/any";

            yield return new CommandSummary(
                commandHref,
                commandTypeId,
                httpContext,
                localContext,
                queueContext,
                BuildRoute(anyContextHref, command.AnyContextRoute),
                BuildWhen(anyContextHref, command.AnyContextWhen));
        }
    }

    IEnumerable<EventSummary> BuildEvents() =>
        from e in _map.Events
        let typeId = GetTypeId(e)
        select new EventSummary(
            $"/events/{typeId}",
            typeId,
            e.Reports.Select(GetTypeId).ToList(),
            e.Workflows.Select(GetTypeId).ToList(),
            e.Handlers.Select(GetTypeId).ToList());

    IEnumerable<EventHandlerSummary> BuildEventHandlers() =>
        from eventHandler in _map.EventHandlers
        let typeId = GetTypeId(eventHandler)
        let eventTypeId = GetTypeId(eventHandler.Event)
        let serviceTypeId = GetTypeId(eventHandler.ServiceType)
        select new EventHandlerSummary($"/events/{eventTypeId}/handlers/{typeId}", typeId, serviceTypeId);

    IEnumerable<QuerySummary> BuildQueries()
    {
        foreach(var query in _map.Queries)
        {
            var queryTypeId = GetTypeId(query);
            var queryHref = $"/queries/{queryTypeId}";

            var httpContext = null as HttpQueryContextSummary;
            var localContext = null as LocalQueryContextSummary;

            foreach(var context in query.Contexts)
            {
                if(context.Info is HttpQueryInfo httpInfo)
                {
                    var contextHref = $"{queryHref}/contexts/http";

                    httpContext = new HttpQueryContextSummary(
                        contextHref,
                        GetTypeId(context.ContextType),
                        BuildHttpRequest(contextHref, httpInfo.Request),
                        BuildQueryResult(contextHref, httpInfo.Result));
                }
                else if(context.Info is LocalQueryInfo localInfo)
                {
                    var contextHref = $"{queryHref}/contexts/local";

                    localContext = new LocalQueryContextSummary(
                        contextHref,
                        GetTypeId(localInfo.Result.DeclaredType),
                        BuildQueryResult(contextHref, localInfo.Result));
                }
                else
                {
                    throw new NotSupportedException($"Query info type {context.Info.DeclaredType} is not supported");
                }
            }

            yield return new QuerySummary(queryHref, queryTypeId, httpContext, localContext);
        }
    }

    IEnumerable<QueryHandlerSummary> BuildQueryHandlers() =>
        from queryHandler in _map.QueryHandlers
        let typeId = GetTypeId(queryHandler)
        let queryTypeId = GetTypeId(queryHandler.Query)
        select new QueryHandlerSummary(
            $"/queries/{queryTypeId}/handlers/{typeId}",
            typeId,
            queryHandler.ServiceTypes.Select(GetTypeId).ToList());

    IEnumerable<ReportSummary> BuildReports() =>
        from report in _map.Reports
        let typeId = GetTypeId(report)
        let href = $"/reports/{typeId}"
        let observations =
            from observation in report.Observations
            let eventTypeId = GetTypeId(observation.Event)
            select new ObservationSummary(
                $"{href}/events/{eventTypeId}",
                eventTypeId,
                BuildRoute(href, observation.Route),
                BuildGiven(href, observation.Given),
                BuildWhen(href, observation.When))
            select new ReportSummary(
                href,
                typeId,
                observations.ToList(),
                report.Queries.Select(GetTypeId).ToList());

    IEnumerable<TopicSummary> BuildTopics() =>
        from topic in _map.Topics
        let typeId = GetTypeId(topic)
        let href = $"/topics/{typeId}"
        select new TopicSummary(
            href,
            typeId,
            topic.Commands.Select(GetTypeId).ToList(),
            topic.Givens.Select(x => BuildGiven(href, x)!).ToList());

    IEnumerable<WorkflowSummary> BuildWorkflows() =>
        from workflow in _map.Workflows
        let typeId = GetTypeId(workflow)
        let href = $"/workflows/{typeId}"
        let observations =
            from observation in workflow.Observations
            let eventTypeId = GetTypeId(observation.Event)
            select new ObservationSummary(
                $"{href}/events/{eventTypeId}",
                eventTypeId,
                BuildRoute(href, observation.Route),
                BuildGiven(href, observation.Given),
                BuildWhen(href, observation.When))
        select new WorkflowSummary(href, typeId, observations.ToList());

    ParameterSummary BuildParameter(string methodHref, TimelineMethodParameter parameter)
    {
        var name = parameter.Info.Name ?? "";

        return new(
            $"{methodHref}/parameter",
            name,
            GetTypeId(parameter.Info.ParameterType),
            GetTypeId(parameter.Message));
    }

    TopicRouteSummary? BuildRoute(string contextHref, TopicRouteMethod? route)
    {
        var href = $"{contextHref}/route";

        return route is null ? null : new(href, route.Info.Name, BuildParameter(href, route.Parameter));
    }

    ObserverRouteSummary BuildRoute(string contextHref, ObserverRouteMethod route)
    {
        var href = $"{contextHref}/route";

        return new(
            href,
            route.Info.Name,
            BuildParameter(href, route.Parameter),
            route.Parameter.HasContext,
            route.ReturnsMany);
    }

    GivenSummary? BuildGiven(string baseHref, GivenMethod? given)
    {
        var href = $"{baseHref}/given";

        return given is null ? null : new(href, given.Info.Name, BuildParameter(href, given.Parameter));
    }

    TopicWhenSummary? BuildWhen(string contextHref, TopicWhenMethod? when)
    {
        var href = $"{contextHref}/when";

        return when is null ? null : new(
            href,
            when.Info.Name,
            BuildParameter(href, when.Parameter),
            when.IsAsync,
            when.HasCancellationToken);
    }

    ObserverWhenSummary? BuildWhen(string contextHref, ObserverWhenMethod? when)
    {
        var href = $"{contextHref}/when";

        return when is null ? null : new(
            href,
            when.Info.Name,
            BuildParameter(href, when.Parameter),
            when.Parameter.HasContext);
    }

    static HttpRequestSummary BuildHttpRequest(string contextHref, HttpRequestInfo info) =>
        new($"{contextHref}/request", info.Method, info.Route);

    QueryResultSummary BuildQueryResult(string contextHref, QueryResult result) =>
        new($"{contextHref}/result",
            GetTypeId(result.DeclaredType),
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
            _types.Add(new(
                $"/types/{id}",
                id,
                type.Namespace ?? "",
                type.Name,
                type.FullName ?? "",
                assemblyQualifiedName));
        }

        return id;
    }

    Id GetTypeId(MapType type) =>
        GetTypeId(type.DeclaredType);
}
