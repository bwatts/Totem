using System.Reflection;
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
        var routes = new List<ObserverRouteMethod>();
        var givens = new List<GivenMethod>();
        var whens = new List<ObserverWhenMethod>();
        var details = new List<IMapCheck>();

        foreach(var method in declaredType.GetRuntimeMethods())
        {
            if(method.IsConstructor)
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

            var givenResult = map.CheckGiven(method);

            details.Add(givenResult);

            if(givenResult)
            {
                givens.Add(givenResult);
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
            return new(declaredType, $"Expected {TimelineMethod.Route}/{TimelineMethod.When} methods for at least one event", details);
        }

        var report = new ReportType(declaredType);

        foreach(var (e, route, given, when) in observations)
        {
            if(route is null)
            {
                return new(declaredType, $"Expected a {TimelineMethod.Route} method for event {e}", details);
            }

            if(given is null && when is null)
            {
                return new(declaredType, $"Expected {TimelineMethod.Given} and/or {TimelineMethod.When} methods for event {e}", details);
            }

            report.Observations.Add(new(e, route, given, when));

            e.Reports.Add(report);
        }

        return new(declaredType, report, details);
    }
}
