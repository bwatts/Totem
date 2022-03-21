using System.Reflection;
using Totem.Core;

namespace Totem.Map.Builder;

internal static class ObserverReflection
{
    internal static MapCheck<ObserverConstructor> CheckObserverConstructor(this Type declaredType)
    {
        var constructors = declaredType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic);

        if(constructors.Length == 0)
        {
            return new(declaredType, new ObserverConstructor(declaredType));
        }

        var constructor = constructors.FirstOrDefault(x => x.GetParameters().Length == 0);

        return constructor is null
            ? new(declaredType, "a parameterless constructor")
            : new(declaredType, new ObserverConstructor(constructor));
    }

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

        var parameterResult = map.CheckObserverRouteParameter(parameters[0]);

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
        if(!method.IsFamily)
        {
            return new(method, "protected accessibility");
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
            return new(method, "a single parameter");
        }

        var parameterResult = map.CheckObserverWhenParameter(parameters[0]);

        if(!parameterResult.HasOutput)
        {
            return new(method, "an event or event context parameter", parameterResult);
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

    static MapCheck<ObserverRouteParameter> CheckObserverRouteParameter(this RuntimeMap map, ParameterInfo parameter)
    {
        var parameterType = parameter.ParameterType;

        if(typeof(IEvent).IsAssignableFrom(parameterType))
        {
            var e = map.GetOrAddEvent(parameterType);

            return new(parameter, new ObserverRouteParameter(parameter, e));
        }

        return new(parameter, $"a parameter assignable to {typeof(IEvent)}");
    }

    static MapCheck<ObserverWhenParameter> CheckObserverWhenParameter(this RuntimeMap map, ParameterInfo parameter)
    {
        var parameterType = parameter.ParameterType;

        if(typeof(IEvent).IsAssignableFrom(parameterType))
        {
            var e = map.GetOrAddEvent(parameterType);

            return new(parameter, new ObserverWhenParameter(parameter, e, hasContext: false));
        }

        var eventType = parameterType.GetImplementedInterfaceGenericArguments(typeof(IEventContext<>)).FirstOrDefault();

        if(eventType is not null)
        {
            var e = map.GetOrAddEvent(eventType);

            return new(parameter, new ObserverWhenParameter(parameter, e, hasContext: true));
        }

        return new(parameter, $"a parameter assignable to {typeof(IEvent)} or {typeof(IEventContext<>)}");
    }
}
