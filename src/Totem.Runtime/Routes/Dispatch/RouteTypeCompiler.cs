using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Totem.Core;

namespace Totem.Routes.Dispatch
{
    internal class RouteTypeCompiler
    {
        const string Route = nameof(Route);
        const string Given = nameof(Given);
        const string When = nameof(When);
        const string WhenAsync = nameof(WhenAsync);

        readonly IServiceProvider _services;
        readonly ILogger _logger;
        readonly Type _routeType;
        readonly Func<Id, IRouteSubscriber> _createSubscriber;

        internal RouteTypeCompiler(IServiceProvider services, ILogger logger, Type routeType, Func<Id, IRouteSubscriber> createSubscriber)
        {
            _services = services;
            _logger = logger;
            _routeType = routeType;
            _createSubscriber = createSubscriber;

            CompileRoutes();
            CompileWhens();
            ValidateEvents();
        }

        internal Dictionary<Type, RouteMethod> RoutesByEventType { get; } = new();
        internal Dictionary<Type, WhenMethod> WhensByEventType { get; } = new();

        void CompileRoutes()
        {
            foreach(var method in _routeType
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                .Where(method => method.Name == Route))
            {
                if(!TryGetEventType(method, out var eventType))
                {
                    _logger.LogWarning("{Route} method does not accept a lone event parameter: {@RouteType}.{Method}", Route, _routeType, method);
                }
                else if(!CheckRouteReturn(method))
                {
                    _logger.LogWarning("{Route} method does not return {IdType}, {IdsType}, or a type assignable to {EnumerableType}: {@RouteType}.{Method}", Route, typeof(Id), typeof(Ids), typeof(IEnumerable<Id>), _routeType, method);
                }
                else if(!method.IsPublic)
                {
                    _logger.LogWarning("{Route} method is not public: {@RouteType}.{Method}", Route, _routeType, method);
                }
                else if(!method.IsStatic)
                {
                    _logger.LogWarning("{Route} method is not static: {@RouteType}.{Method}", Route, _routeType, method);
                }
                else if(RoutesByEventType.ContainsKey(eventType))
                {
                    _logger.LogWarning("{Route} method duplicates another signature for event {Eventtype}: {@RouteType}.{Method}", Route, eventType, _routeType, method);
                }
                else
                {
                    RoutesByEventType[eventType] = new RouteMethod(_routeType, eventType, method, _createSubscriber);
                }
            }
        }

        static bool TryGetEventType(MethodInfo method, out Type eventType)
        {
            var parameters = method.GetParameters();

            if(parameters.Length == 1 && typeof(IEvent).IsAssignableFrom(parameters[0].ParameterType))
            {
                eventType = parameters[0].ParameterType;
                return true;
            }

            eventType = null!;
            return false;
        }

        static bool CheckRouteReturn(MethodInfo method) =>
            method.ReturnType == typeof(Id) || typeof(IEnumerable<Id>).IsAssignableFrom(method.ReturnType);

        void CompileWhens()
        {
            foreach(var method in _routeType
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                .Where(method => method.Name == When || method.Name == WhenAsync))
            {
                if(!TryGetWhenEvent(method, out var eventType, out var isContext))
                {
                    _logger.LogWarning("{MethodName} method does not accept an event parameter first: {@RouteType}.{Method}", method.Name, _routeType, method);
                }
                else if(!TryGetIsAsync(method, out var isAsync))
                {
                    _logger.LogWarning("{MethodName} method does not return {TaskType} or {VoidType}: {@RouteType}.{Method}", method.Name, typeof(Task), typeof(void), _routeType, method);
                }
                else if(!TryGetHasCancellationToken(method, out var hasCancellationToken))
                {
                    _logger.LogWarning("{MethodName} methods only accept a single optional parameter of type {CancellationTokenType} after the event: {@RouteType}.{Method}", method.Name, typeof(CancellationToken), _routeType, method);
                }
                else if(!method.IsPublic)
                {
                    _logger.LogWarning("{MethodName} method is not public: {@RouteType}.{Method}", method.Name, _routeType, method);
                }
                else if(method.IsStatic)
                {
                    _logger.LogWarning("{MethodName} method is static: {@RouteType}.{Method}", method.Name, _routeType, method);
                }
                else if(WhensByEventType.ContainsKey(eventType))
                {
                    _logger.LogWarning("{MethodName} method duplicates another signature for event {EventType}: {@RouteType}.{Method}", method.Name, eventType, _routeType, method);
                }
                else
                {
                    WhensByEventType[eventType] = new WhenMethod(method, eventType, isContext, isAsync, hasCancellationToken);
                }
            }
        }

        static bool TryGetWhenEvent(MethodInfo method, out Type eventType, out bool isContext)
        {
            var parameterType = method.GetParameters().FirstOrDefault()?.ParameterType;

            if(parameterType != null)
            {
                if(typeof(IEvent).IsAssignableFrom(parameterType))
                {
                    eventType = parameterType;
                    isContext = false;
                    return true;
                }

                var contextEventType = parameterType.GetInterfaceGenericArguments(typeof(IRouteContext<>)).FirstOrDefault();

                if(contextEventType != null)
                {
                    eventType = contextEventType;
                    isContext = true;
                    return true;
                }
            }

            eventType = null!;
            isContext = false;
            return false;
        }

        static bool TryGetIsAsync(MethodInfo method, out bool isAsync)
        {
            if(method.ReturnType == typeof(Task))
            {
                isAsync = true;
                return true;
            }

            if(method.ReturnType == typeof(void))
            {
                isAsync = false;
                return true;
            }

            isAsync = false;
            return false;
        }

        static bool TryGetHasCancellationToken(MethodInfo method, out bool hasCancellationToken)
        {
            var parameters = method.GetParameters();

            if(parameters.Length == 1)
            {
                hasCancellationToken = false;
                return true;
            }

            if(parameters.Length == 2 && parameters[1].ParameterType == typeof(CancellationToken))
            {
                hasCancellationToken = true;
                return true;
            }

            hasCancellationToken = false;
            return false;
        }

        void ValidateEvents()
        {
            var givenTypes = GetGivenTypes().ToHashSet();

            foreach(var eventType in RoutesByEventType.Keys.Union(WhensByEventType.Keys).Union(givenTypes))
            {
                var hasRoute = RoutesByEventType.ContainsKey(eventType);
                var hasGiven = givenTypes.Contains(eventType);
                var hasWhen = WhensByEventType.ContainsKey(eventType);

                if(hasRoute && (hasGiven || hasWhen))
                {
                    continue;
                }

                if(hasRoute)
                {
                    _logger.LogWarning("{@RouteType}.{Route} for event {EventType} has no corresponding {Given}, {When}, or {WhenAsync} method", _routeType, Route, eventType, Given, When, WhenAsync);

                    RoutesByEventType.Remove(eventType);
                }
                else if(hasWhen)
                {
                    var whenMethod = WhensByEventType[eventType].Info;

                    _logger.LogWarning("{@RouteType}.{Method} for event {EventType} has no corresponding {Route} method", _routeType, whenMethod, eventType, Route);

                    WhensByEventType.Remove(eventType);
                }
                else
                {
                    _logger.LogWarning("{@RouteType}.{Given} for event {EventType} has no corresponding {Route} method", _routeType, Given, eventType, Route);
                }
            }
        }

        IEnumerable<Type> GetGivenTypes()
        {
            using var scope = _services.CreateScope();

            var service = scope.ServiceProvider.GetService(_routeType);

            if(service == null)
                throw new InvalidOperationException($"Route type {_routeType} is not available from the service provider. Add the service explicitly, or use AddProjectionsAsServices/AddWorkflowsAsServices to discover them.");

            if(service is not IRoute route)
                throw new InvalidOperationException($"Route type {_routeType} does not implement {typeof(IRoute)}");

            return route.GivenTypes;
        }
    }
}