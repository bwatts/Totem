using System.Linq.Expressions;
using System.Reflection;
using Totem.Core;
using Totem.Http;
using Totem.Local;
using Totem.Queues;
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
        var topic = new TopicType(declaredType);
        var routes = new List<TopicRouteMethod>();
        var whens = new List<TopicWhenMethod>();
        var details = new List<IMapCheck>();

        foreach(var method in declaredType.GetRuntimeMethods())
        {
            if(method.DeclaringType?.Assembly == Assembly.GetExecutingAssembly())
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

            var givenResult = map.CheckTimelineGiven(method);

            details.Add(givenResult);

            if(givenResult)
            {
                topic.Givens.Add(givenResult);
                continue;
            }

            var whenResult = map.CheckTopicWhen(method);

            details.Add(whenResult);

            if(whenResult)
            {
                whens.Add(whenResult);
            }
        }

        if(routes.Count == 0)
        {
            topic.SetSingleInstanceId();
        }

        var commandTypes = (
            from command in routes.Select(x => x.Parameter.Message).Union(whens.Select(x => x.Parameter.Message))
            join route in routes on command equals route.Parameter.Message into commandRoutes
            join when in whens on command equals when.Parameter.Message into commandWhens
            select (command, commandRoutes.FirstOrDefault(), commandWhens.ToList()))
            .ToList();

        if(commandTypes.Count == 0)
        {
            return new(declaredType, $"{TimelineMethod.Route}/{TimelineMethod.When} methods for at least one command", details);
        }

        foreach(var (commandType, commandRoute, commandWhens) in commandTypes)
        {
            if(routes.Count > 0 && commandRoute is null)
            {
                return new(declaredType, $"a {TimelineMethod.Route} method for command {commandType}", details);
            }

            commandType.Topic = topic;
            commandType.Route = commandRoute;

            foreach(var when in commandWhens)
            {
                if(when.Parameter.ContextType is null)
                {
                    commandType.WhenWithoutContext = when;
                    continue;
                }

                var httpContextResult = when.CheckHttpContext();

                details.Add(httpContextResult);

                if(httpContextResult)
                {
                    commandType.HttpContext = httpContextResult;
                    continue;
                }

                var localContextResult = when.CheckLocalContext();

                details.Add(localContextResult);

                if(localContextResult)
                {
                    commandType.LocalContext = localContextResult;
                    continue;
                }

                var queueContextResult = when.CheckQueueContext();

                details.Add(queueContextResult);

                if(queueContextResult)
                {
                    commandType.QueueContext = queueContextResult;
                }
            }

            if(commandType.WhenWithoutContext is null
                && commandType.HttpContext is null
                && commandType.LocalContext is null
                && commandType.QueueContext is null)
            {
                return new(declaredType, $"at least one {TimelineMethod.When} method", details);
            }

            topic.Commands.Add(commandType);
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

        var parameterResult = map.CheckTopicRouteParameter(parameters[0]);

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
        if(!method.IsFamily)
        {
            return new(method, "protected accessibility");
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

        var parameterResult = map.CheckTopicWhenParameter(parameters[0]);

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

    static MapCheck<TopicRouteParameter> CheckTopicRouteParameter(this RuntimeMap map, ParameterInfo parameter)
    {
        var parameterType = parameter.ParameterType;

        if(typeof(ICommandMessage).IsAssignableFrom(parameterType))
        {
            var command = map.GetOrAddCommand(parameterType);

            return new(parameter, new TopicRouteParameter(parameter, command));
        }

        return new(parameter, $"a parameter assignable to {typeof(ICommandMessage)}");
    }

    static MapCheck<TopicWhenParameter> CheckTopicWhenParameter(this RuntimeMap map, ParameterInfo parameter)
    {
        var parameterType = parameter.ParameterType;

        if(typeof(ICommandMessage).IsAssignableFrom(parameterType))
        {
            var command = map.GetOrAddCommand(parameterType);

            return new(parameter, new TopicWhenParameter(parameter, command, contextType: null));
        }

        var commandType = parameterType.GetImplementedInterfaceGenericArguments(typeof(ICommandContext<>)).SingleOrDefault();

        if(commandType is not null)
        {
            var command = map.GetOrAddCommand(commandType);

            return new(parameter, new TopicWhenParameter(parameter, command, contextType: parameterType));
        }

        return new(parameter, $"a parameter assignable to {typeof(ICommandMessage)} or {typeof(ICommandContext<>)}");
    }

    static MapCheck<TopicWhenContext> CheckHttpContext(this TopicWhenMethod when)
    {
        var commandType = when.Parameter.Message.DeclaredType;
        var interfaceType = typeof(IHttpCommandContext<>).MakeGenericType(commandType);

        if(when.Parameter.ContextType != interfaceType)
        {
            return new(when, $"parameter of type {interfaceType}");
        }

        if(!HttpCommandInfo.TryFrom(commandType, out var httpInfo))
        {
            return new(when, $"command type to implement {typeof(IHttpCommand)}");
        }

        return new(when, new TopicWhenContext(interfaceType, httpInfo, when));
    }

    static MapCheck<TopicWhenContext> CheckLocalContext(this TopicWhenMethod when)
    {
        var commandType = when.Parameter.Message.DeclaredType;
        var interfaceType = typeof(ILocalCommandContext<>).MakeGenericType(commandType);

        if(when.Parameter.ContextType != interfaceType)
        {
            return new(when, $"parameter of type {interfaceType}");
        }

        if(!LocalCommandInfo.TryFrom(commandType, out var localInfo))
        {
            return new(when, $"command type to implement {typeof(ILocalCommand)}");
        }

        return new(when, new TopicWhenContext(interfaceType, localInfo, when));
    }

    static MapCheck<TopicWhenContext> CheckQueueContext(this TopicWhenMethod when)
    {
        var commandType = when.Parameter.Message.DeclaredType;
        var interfaceType = typeof(IQueueCommandContext<>).MakeGenericType(commandType);

        if(when.Parameter.ContextType != interfaceType)
        {
            return new(when, $"parameter of type {interfaceType}");
        }

        if(!QueueCommandInfo.TryFrom(commandType, out var queueInfo))
        {
            return new(when, $"command type to implement {typeof(IQueueCommand)}");
        }

        return new(when, new TopicWhenContext(interfaceType, queueInfo, when));
    }
}
