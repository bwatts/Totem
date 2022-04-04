using System.Reflection;
using Totem.Core;
using Totem.Reports;

namespace Totem.Map.Builder;

internal static class ReportReflection
{
    internal static bool TryAddReport(this RuntimeMap map, Type declaredType)
    {
        if(!typeof(IReport).IsAssignableFrom(declaredType))
        {
            return false;
        }

        var result = map.CheckReport(declaredType);

        map.AddCheck(result);

        if(result)
        {
            map.Reports.Add(result);
        }

        return result;
    }

    static MapCheck<ReportType> CheckReport(this RuntimeMap map, Type declaredType)
    {
        var constructorResult = declaredType.CheckObserverConstructor();

        if(!constructorResult)
        {
            return new(declaredType, "a constructor", constructorResult);
        }

        var rowResult = map.CheckReportRow(declaredType);

        if(!rowResult)
        {
            return new(declaredType, "a dedicated report row", rowResult);
        }

        var routes = new List<ObserverRouteMethod>();
        var givens = new List<GivenMethod>();
        var whens = new List<ObserverWhenMethod>();
        var details = new List<IMapCheck>();

        foreach(var method in declaredType.GetRuntimeMethods())
        {
            if(method.DeclaringType?.Assembly == Assembly.GetExecutingAssembly())
            {
                continue;
            }

            var routeResult = map.CheckObserverRoute(method);

            details.Add(routeResult);

            if(routeResult)
            {
                routes.Add(routeResult);
                continue;
            }

            var givenResult = map.CheckTimelineGiven(method);

            details.Add(givenResult);

            if(givenResult)
            {
                givens.Add(givenResult);
                continue;
            }

            var whenResult = map.CheckObserverWhen(method);

            details.Add(whenResult);

            if(whenResult)
            {
                whens.Add(whenResult);
            }
        }

        var eventTypes = routes
            .Select(x => x.Parameter.Message)
            .Union(whens.Select(x => x.Parameter.Message));

        var observations = (
            from eventType in eventTypes
            join route in routes on eventType equals route.Parameter.Message into eventRoutes
            join given in givens on eventType equals given.Parameter.Message into eventGivens
            join when in whens on eventType equals when.Parameter.Message into eventWhens
            select (eventType, eventRoutes.SingleOrDefault(), eventGivens.SingleOrDefault(), eventWhens.SingleOrDefault()))
            .ToList();

        if(observations.Count == 0)
        {
            return new(declaredType, $"{TimelineMethod.Route}/{TimelineMethod.When} methods for at least one event", details);
        }

        var report = new ReportType(declaredType, constructorResult, rowResult);

        foreach(var (e, route, given, when) in observations)
        {
            if(route is null)
            {
                return new(declaredType, $"a {TimelineMethod.Route} method for event {e}", details);
            }

            if(given is null && when is null)
            {
                return new(declaredType, $"{TimelineMethod.Given} and/or {TimelineMethod.When} method for event {e}", details);
            }

            report.Observations.Add(new(e, route, given, when));

            e.Reports.Add(report);
        }

        return new(declaredType, report, details);
    }

    static MapCheck<ReportRowType> CheckReportRow(this RuntimeMap map, Type declaredType)
    {
        var baseType = declaredType.BaseType;

        if(baseType is null || !baseType.IsGenericTypeDefinition(typeof(Report<>)))
        {
            return new(declaredType, $"type to derive from {typeof(Report<>)}");
        }

        var rowType = baseType.GetGenericArguments().Single();

        if(map.ReportRows.TryGet(rowType, out var existingRow))
        {
            return new(declaredType, $"row to be exclusive to this report");
        }

        var properties = declaredType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(x => new ReportRowProperty(x))
            .ToList();

        return new(declaredType, new ReportRowType(rowType, properties));
    }
}
