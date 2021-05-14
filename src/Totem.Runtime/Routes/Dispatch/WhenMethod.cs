using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Routes.Dispatch
{
    internal class WhenMethod
    {
        delegate Task InvokeAsync(IRoute route, IRouteContext<IEvent> context, CancellationToken cancellationToken);
        delegate void Invoke(IRoute route, IRouteContext<IEvent> context, CancellationToken cancellationToken);

        readonly InvokeAsync _invokeAsync = null!;
        readonly Invoke _invoke = null!;

        public WhenMethod(MethodInfo info, Type eventType, bool isContext, bool isAsync, bool hasCancellationToken)
        {
            Info = info;

            // (route, context, cancellationToken) => ((TRoute) route).method((IRouteContext<TEvent>) context, cancellationToken)
            // (route, context, cancellationToken) => ((TRoute) route).method((IRouteContext<TEvent>) context)
            // (route, context, cancellationToken) => ((TRoute) route).method((TEvent) context.Event, cancellationToken)
            // (route, context, cancellationToken) => ((TRoute) route).method((TEvent) context.Event)

            var routeParameter = Expression.Parameter(typeof(IRoute), "route");
            var contextParameter = Expression.Parameter(typeof(IRouteContext<IEvent>), "context");
            var cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

            var argument = isContext
                ? Expression.Convert(contextParameter, typeof(IRouteContext<>).MakeGenericType(eventType))
                : Expression.Convert(Expression.Property(contextParameter, nameof(IRouteContext<IEvent>.Event)), eventType);

            var typedRoute = Expression.Convert(routeParameter, info.DeclaringType!);

            var call = hasCancellationToken
                ? Expression.Call(typedRoute, info, argument, cancellationTokenParameter)
                : Expression.Call(typedRoute, info, argument);

            if(isAsync)
            {
                _invokeAsync = Expression.Lambda<InvokeAsync>(call, routeParameter, contextParameter, cancellationTokenParameter).Compile();
            }
            else
            {
                _invoke = Expression.Lambda<Invoke>(call, routeParameter, contextParameter, cancellationTokenParameter).Compile();
            }
        }

        internal MethodInfo Info { get; }

        internal Task CallAsync(IRoute route, IRouteContext<IEvent> context, CancellationToken cancellationToken)
        {
            if(_invokeAsync != null)
            {
                return _invokeAsync(route, context, cancellationToken);
            }

            _invoke(route, context, cancellationToken);

            return Task.CompletedTask;
        }
    }
}