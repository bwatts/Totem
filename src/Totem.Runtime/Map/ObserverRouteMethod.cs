using System.Linq.Expressions;
using System.Reflection;
using Totem.Core;

namespace Totem.Map;

public class ObserverRouteMethod : TimelineMethod
{
    readonly Func<IEvent, Id>? _call;
    readonly Func<IEvent, IEnumerable<Id>>? _callMany;

    internal ObserverRouteMethod(MethodInfo info, ObserverRouteParameter parameter, bool returnsMany) : base(info, parameter)
    {
        ReturnsMany = returnsMany;

        var eventParameter = Expression.Parameter(typeof(IEvent), "e");
        var call = Expression.Call(info, parameter.ToExpression(eventParameter));

        if(returnsMany)
        {
            var lambda = Expression.Lambda<Func<IEvent, IEnumerable<Id>>>(call, eventParameter);

            _callMany = lambda.Compile();
        }
        else
        {
            var lambda = Expression.Lambda<Func<IEvent, Id>>(call, eventParameter);

            _call = lambda.Compile();
        }
    }

    public new ObserverRouteParameter Parameter => (ObserverRouteParameter) base.Parameter;
    public bool ReturnsMany { get; }

    internal IEnumerable<ItemKey> Call(IEvent e)
    {
        var observerType = Info.DeclaringType!;

        if(_call is not null)
        {
            yield return new(observerType, _call(e));
        }
        else
        {
            foreach(var id in _callMany!(e))
            {
                yield return new(observerType, id);
            }
        }
    }
}
