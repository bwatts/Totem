using System.Reflection;
using Totem.Core;

namespace Totem.Map.Builder;

internal static class ObserverReflection
{
    internal static MapCheck<ObserverRouteMethod> CheckObserverRoute(this RuntimeMap map, MethodInfo method)
    {
        if(!method.IsPublic)
        {
            return new(method, "public accessibility");
        }

        if(!method.IsStatic)
        {
            return new(method, "a static scope");
        }

        var returnsMany = typeof(IEnumerable<Id>).IsAssignableFrom(method.ReturnType);

        if(method.ReturnType != typeof(Id) && !returnsMany)
        {
            return new(method, $"a return type of {typeof(Id)} or assignable to {typeof(IEnumerable<Id>)}");
        }

        var parameters = method.GetParameters();

        if(parameters.Length != 1)
        {
            return new(method, "a single parameter");
        }

        var parameterResult = map.CheckObserverParameter(parameters[0]);

        if(!parameterResult)
        {
            return new(method, "an event parameter", parameterResult);
        }

        if(method.Name != TimelineMethod.Route)
        {
            var expected = TypoDetector.IsPossibleTypo(TimelineMethod.Route, method.Name)
                ? $"a name of \"{TimelineMethod.Route}\" which is similar to \"{method.Name}\" [POSSIBLE TYPO]"
                : $"a name of \"{TimelineMethod.Route}\"";

            return new(method, expected, parameterResult);
        }

        return new(method, new ObserverRouteMethod(method, parameterResult, returnsMany), parameterResult);
    }

    internal static MapCheck<ObserverWhenMethod> CheckObserverWhen(this RuntimeMap map, MethodInfo method)
    {
        if(!method.IsPublic)
        {
            return new(method, "public accessibility");
        }

        if(method.IsStatic)
        {
            return new(method, "an instance scope");
        }

        if(method.ReturnType != typeof(void))
        {
            return new(method, $"a return type of {typeof(void)}");
        }

        var parameters = method.GetParameters();

        if(parameters.Length != 1)
        {
            return new(method, "Expected a single parameter");
        }

        var parameterResult = map.CheckObserverParameter(parameters[0]);

        if(!parameterResult.HasOutput)
        {
            return new(method, "an event parameter", parameterResult);
        }

        if(method.Name != TimelineMethod.When)
        {
            var expected = TypoDetector.IsPossibleTypo(TimelineMethod.When, method.Name)
                ? $"a name of \"{TimelineMethod.When}\" which is similar to \"{method.Name}\" [POSSIBLE TYPO]"
                : $"a name of \"{TimelineMethod.When}\"";

            return new(method, expected, parameterResult);
        }

        return new(method, new ObserverWhenMethod(method, parameterResult));
    }

    static MapCheck<ObserverMethodParameter> CheckObserverParameter(this RuntimeMap map, ParameterInfo parameter)
    {
        var parameterType = parameter.ParameterType;

        if(typeof(IEvent).IsAssignableFrom(parameterType))
        {
            if(map.Events.TryGet(parameterType, out var knownType) && knownType is EventType e)
            {
                return new(parameter, new ObserverMethodParameter(parameter, e, hasContext: false));
            }

            return new(parameter, $"a known event of type {parameterType}");
        }

        var eventType = parameterType.GetImplementedInterfaceGenericArguments(typeof(IEventContext<>)).FirstOrDefault();

        if(eventType is not null)
        {
            if(map.Events.TryGet(eventType, out var knownType) && knownType is EventType e)
            {
                return new(parameter, new ObserverMethodParameter(parameter, e, hasContext: true));
            }

            return new(parameter, $"a known event of type {eventType}");
        }

        return new(parameter, $"a parameter of type {typeof(IEventContext<>)} or assignable to {typeof(IEvent)}");
    }
}
