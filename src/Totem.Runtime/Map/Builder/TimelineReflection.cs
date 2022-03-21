using System.Reflection;

namespace Totem.Map.Builder;

internal static class TimelineReflection
{
    internal static MapCheck<GivenMethod> CheckTimelineGiven(this RuntimeMap map, MethodInfo method)
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

        var parameterResult = map.CheckGivenParameter(parameters[0]);

        if(!parameterResult)
        {
            return new(method, "an event parameter", parameterResult);
        }

        if(method.Name != TimelineMethod.Given)
        {
            var expected = TypoDetector.IsPossibleTypo(TimelineMethod.Given, method.Name)
                ? $"a name of \"{TimelineMethod.Given}\" which is similar to \"{method.Name}\" [POSSIBLE TYPO]"
                : $"a name of \"{TimelineMethod.Given}\"";

            return new(method, expected, parameterResult);
        }

        return new(method, new GivenMethod(method, parameterResult), parameterResult);
    }

    static MapCheck<GivenMethodParameter> CheckGivenParameter(this RuntimeMap map, ParameterInfo parameter)
    {
        var parameterType = parameter.ParameterType;

        if(typeof(IEvent).IsAssignableFrom(parameterType))
        {
            var e = map.GetOrAddEvent(parameterType);

            return new(parameter, new GivenMethodParameter(parameter, e));
        }

        return new(parameter, $"a parameter assignable to {typeof(IEvent)}");
    }
}