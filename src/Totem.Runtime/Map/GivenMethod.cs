using System.Linq.Expressions;
using System.Reflection;
using Totem.Core;

namespace Totem.Map;

public class GivenMethod : TimelineMethod
{
    internal GivenMethod(MethodInfo info, GivenMethodParameter parameter) : base(info, parameter)
    {
        // (timeline, e) => ((TTimeline) timeline).info((TEvent) e)

        var timelineParameter = Expression.Parameter(typeof(ITimeline), "timeline");
        var eventParameter = Expression.Parameter(typeof(IEvent), "e");
        var timelineCast = Expression.Convert(timelineParameter, info.DeclaringType!);
        var eventCast = Expression.Convert(eventParameter, parameter.Message.DeclaredType);
        var call = Expression.Call(timelineCast, info, eventCast);
        var lambda = Expression.Lambda<Action<ITimeline, IEvent>>(call, timelineParameter, eventParameter);

        Call = lambda.Compile();
    }

    public new GivenMethodParameter Parameter => (GivenMethodParameter) base.Parameter;
    internal Action<ITimeline, IEvent> Call { get; }
}
