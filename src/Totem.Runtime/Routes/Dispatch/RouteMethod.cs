using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Routes.Dispatch
{
    internal class RouteMethod
    {
        delegate IEnumerable<IRouteSubscriber> Invoke(IEvent e);

        readonly Type _routeType;
        readonly Invoke _invoke;
        readonly Func<Id, IRouteSubscriber> _createSubscriber;

        internal RouteMethod(Type routeType, Type eventType, MethodInfo info, Func<Id, IRouteSubscriber> createSubscriber)
        {
            _routeType = routeType;
            _createSubscriber = createSubscriber;

            // e => CreateSubscribers(Info((TEvent) e))

            var eventParameter = Expression.Parameter(typeof(IEvent), "e");
            var callMethod = Expression.Call(info, Expression.Convert(eventParameter, eventType));
            var callCreateSubscribers = Expression.Call(Expression.Constant(this), nameof(CreateSubscribers), null, callMethod);

            _invoke = Expression.Lambda<Invoke>(callCreateSubscribers, eventParameter).Compile();
        }

        internal IEnumerable<IRouteSubscriber> Call(IEvent e) =>
            _invoke(e);

        IEnumerable<IRouteSubscriber> CreateSubscribers(Id selectedId) =>
            new[] { CreateSubscriber(selectedId) };

        IEnumerable<IRouteSubscriber> CreateSubscribers(Ids selectedIds) =>
            selectedIds.Select(CreateSubscriber);

        IEnumerable<IRouteSubscriber> CreateSubscribers(IEnumerable<Id> selectedIds) =>
            selectedIds.Select(CreateSubscriber);

        IRouteSubscriber CreateSubscriber(Id selectedId) =>
            _createSubscriber(selectedId.DeriveId(_routeType.FullName!));
    }
}