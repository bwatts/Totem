using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Totem.Core;

namespace Totem.Map;

public class ObserverRouteMethod : ObserverMethod
{
    readonly Func<IEventContext<IEvent>, Id>? _call;
    readonly Func<IEventContext<IEvent>, IEnumerable<Id>>? _callMany;

    internal ObserverRouteMethod(MethodInfo info, ObserverMethodParameter parameter, bool returnsMany) : base(info, parameter)
    {
        ReturnsMany = returnsMany;

        var contextParameter = Expression.Parameter(typeof(IEventContext<IEvent>), "context");
        var call = Expression.Call(info, parameter.ToArgument(contextParameter));

        if(returnsMany)
        {
            var lambda = Expression.Lambda<Func<IEventContext<IEvent>, IEnumerable<Id>>>(call, contextParameter);

            _callMany = lambda.Compile();
        }
        else
        {
            var lambda = Expression.Lambda<Func<IEventContext<IEvent>, Id>>(call, contextParameter);

            _call = lambda.Compile();
        }
    }

    public bool ReturnsMany { get; }

    internal IEnumerable<ItemKey> Call(IEventContext<IEvent> context)
    {
        var observerType = Info.DeclaringType!;

        if(_call is not null)
        {
            yield return new(observerType, _call(context));
        }
        else
        {
            foreach(var id in _callMany!(context))
            {
                yield return new(observerType, id);
            }
        }
    }
}
