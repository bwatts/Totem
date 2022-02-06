using System.Linq.Expressions;
using System.Reflection;
using Totem.Core;

namespace Totem.Map;

public class ObserverWhenMethod : ObserverMethod
{
    internal ObserverWhenMethod(MethodInfo info, ObserverMethodParameter parameter) : base(info, parameter)
    {
        // (timeline, context) => ((TTimeline) timeline).info(<parameter>)

        var timelineParameter = Expression.Parameter(typeof(IObserverTimeline), "timeline");
        var contextParameter = Expression.Parameter(typeof(IEventContext<IEvent>), "context");
        var timelineCast = Expression.Convert(timelineParameter, info.DeclaringType!);
        var call = Expression.Call(timelineCast, info, parameter.ToArgument(contextParameter));
        var lambda = Expression.Lambda<Action<IObserverTimeline, IEventContext<IEvent>>>(call, timelineParameter, contextParameter);

        Call = lambda.Compile();
    }

    internal Action<IObserverTimeline, IEventContext<IEvent>> Call { get; }
}
