using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Totem.Core;
using Totem.Topics;

namespace Totem.Map.Builder;

internal static class TopicReflection
{
    internal static bool TryAddTopic(this RuntimeMap map, Type declaredType)
    {
        if(!typeof(ITopic).IsAssignableFrom(declaredType))
        {
            return false;
        }

        var result = map.CheckTopic(declaredType);

        map.AddCheck(result);

        if(result)
        {
            map.Topics.Add(result);
        }

        return result;
    }

    static MapCheck<TopicType> CheckTopic(this RuntimeMap map, Type declaredType)
    {
        var routes = new List<TopicRouteMethod>();
        var givens = new List<GivenMethod>();
        var whens = new List<TopicWhenMethod>();
        var details = new List<IMapCheck>();

        foreach(var method in declaredType.GetRuntimeMethods())
        {
            if(method.IsConstructor)
            {
                continue;
            }

            var routeResult = map.CheckTopicRoute(method);

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

            var whenResult = map.CheckTopicWhen(method);

            details.Add(whenResult);

            if(whenResult)
            {
                whens.Add(whenResult);
            }
        }

        var commandTypes = (
            from command in routes.Select(x => x.Parameter.Message).Union(whens.Select(x => x.Parameter.Message))
            join route in routes on command equals route.Parameter.Message into commandRoutes
            join when in whens on command equals when.Parameter.Message into commandWhens
            select (command, commandRoutes.ToList(), commandWhens.ToList()))
            .ToList();

        if(commandTypes.Count == 0)
        {
            return new(declaredType, $"Expected {TimelineMethod.Route}/{TimelineMethod.When} methods for at least one command", details);
        }

        var topic = new TopicType(declaredType);

        foreach(var (commandType, commandRoutes, commandWhens) in commandTypes)
        {
            var anyContextRoute = null as TopicRouteMethod;
            var anyContextWhen = null as TopicWhenMethod;
            var routesByContextType = new Dictionary<Type, TopicRouteMethod>();
            var whensByContextType = new Dictionary<Type, TopicWhenMethod>();

            foreach(var route in commandRoutes)
            {
                if(route.Parameter.ContextType is null)
                {
                    anyContextRoute = route;
                }
                else
                {
                    routesByContextType[route.Parameter.ContextType] = route;
                }
            }

            foreach(var when in commandWhens)
            {
                if(when.Parameter.ContextType is null)
                {
                    anyContextWhen = when;
                }
                else
                {
                    whensByContextType[when.Parameter.ContextType] = when;
                }
            }

            if(anyContextRoute is null && routesByContextType.Count == 0)
            {
                return new(declaredType, $"Expected at least one {TimelineMethod.Route} method", details);
            }

            if(anyContextWhen is null && whensByContextType.Count == 0)
            {
                return new(declaredType, $"Expected at least one {TimelineMethod.When} method", details);
            }

            if(anyContextRoute is null
                && anyContextWhen is null
                && (routesByContextType.Count != whensByContextType.Count || routesByContextType.Keys.Except(whensByContextType.Keys).Any()))
            {
                return new(declaredType, $"Expected the same set of context types between {TimelineMethod.Route} and {TimelineMethod.When} methods (or overloads without a context type)", details);
            }

            commandType.AnyContextRoute = anyContextRoute;
            commandType.AnyContextWhen = anyContextWhen;

            foreach(var contextType in routesByContextType.Keys)
            {
                if(!commandType.Contexts.TryGet(contextType, out var context))
                {
                    return new(declaredType, $"Expected command {commandType} to have context {contextType}", details);
                }

                context.Route = routesByContextType[contextType];
            }

            foreach(var contextType in whensByContextType.Keys)
            {
                if(!commandType.Contexts.TryGet(contextType, out var context))
                {
                    return new(declaredType, $"Expected command {commandType} to have context {contextType}", details);
                }

                context.When = whensByContextType[contextType];
            }

            topic.Commands.Add(commandType);
        }

        foreach(var given in givens)
        {
            topic.Givens.Add(given);
        }

        return new(declaredType, topic, details);
    }

    static MapCheck<TopicRouteMethod> CheckTopicRoute(this RuntimeMap map, MethodInfo method)
    {
        if(!method.IsPublic)
        {
            return new(method, "public accessibility");
        }

        if(!method.IsStatic)
        {
            return new(method, "a static scope");
        }

        if(method.ReturnType != typeof(Id))
        {
            return new(method, $"a return type of {typeof(Id)}");
        }

        var parameters = method.GetParameters();

        if(parameters.Length != 1)
        {
            return new(method, "a single parameter");
        }

        var parameterResult = map.CheckTopicParameter(parameters[0]);

        if(!parameterResult)
        {
            return new(method, "a command parameter", parameterResult);
        }

        if(method.Name != TimelineMethod.Route)
        {
            var expected = TypoDetector.IsPossibleTypo(TimelineMethod.Route, method.Name)
                ? $"a name of \"{TimelineMethod.Route}\" which is similar to \"{method.Name}\" [POSSIBLE TYPO]"
                : $"a name of \"{TimelineMethod.Route}\"";

            return new(method, expected, parameterResult);
        }

        return new(method, new TopicRouteMethod(method, parameterResult), parameterResult);
    }

    static MapCheck<TopicWhenMethod> CheckTopicWhen(this RuntimeMap map, MethodInfo method)
    {
        if(!method.IsPublic)
        {
            return new(method, "public accessibility");
        }

        if(method.IsStatic)
        {
            return new(method, "an instance scope");
        }

        var isAsync = false;
        var hasCancellationToken = false;
        var parameters = method.GetParameters();

        if(method.ReturnType == typeof(void))
        {
            if(parameters.Length != 1)
            {
                return new(method, "Expected a single parameter");
            }
        }
        else if(method.ReturnType == typeof(Task))
        {
            isAsync = true;

            switch(parameters.Length)
            {
                case 1:
                    hasCancellationToken = false;
                    break;
                case 2:
                    if(parameters[1].ParameterType != typeof(CancellationToken))
                    {
                        return new(method, $"Expected second parameter to be of type {typeof(CancellationToken)}");
                    }

                    hasCancellationToken = true;
                    break;
                default:
                    return new(method, $"Expected one or two parameters");

            }
        }
        else
        {
            return new(method, $"Expected a return type of {typeof(void)} or {typeof(Task)}");
        }

        var parameterResult = map.CheckTopicParameter(parameters[0]);

        if(!parameterResult)
        {
            return new(method, "a command parameter", parameterResult);
        }

        if(method.Name != TimelineMethod.When)
        {
            var expected = TypoDetector.IsPossibleTypo(TimelineMethod.When, method.Name)
                ? $"a name of \"{TimelineMethod.When}\" which is similar to \"{method.Name}\" [POSSIBLE TYPO]"
                : $"a name of \"{TimelineMethod.When}\"";

            return new(method, expected, parameterResult);
        }

        return new(method, new TopicWhenMethod(method, parameterResult, isAsync, hasCancellationToken), parameterResult);
    }

    static MapCheck<TopicMethodParameter> CheckTopicParameter(this RuntimeMap map, ParameterInfo parameter)
    {
        var parameterType = parameter.ParameterType;

        if(typeof(ICommandMessage).IsAssignableFrom(parameterType))
        {
            return map.Commands.TryGet(parameterType, out var command)
                ? new(parameter, new TopicMethodParameter(parameter, command, contextType: null))
                : new(parameter, $"a known command of type {parameterType}");
        }

        var commandType = parameterType.GetImplementedInterfaceGenericArguments(typeof(ICommandContext<>)).SingleOrDefault();

        if(commandType is not null)
        {
            return map.Commands.TryGet(commandType, out var command)
                ? new(parameter, new TopicMethodParameter(parameter, command, contextType: parameterType))
                : new(parameter, $"a known command of type {commandType}");
        }

        return new(parameter, $"a parameter assignable to {typeof(ICommandMessage)} or {typeof(ICommandContext<>)}");
    }
}
