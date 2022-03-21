using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Map;

public class ObserverRouteParameter : TimelineMethodParameter
{
    internal ObserverRouteParameter(ParameterInfo info, EventType message) : base(info, message)
    { }

    public new EventType Message => (EventType) base.Message;

    internal Expression ToExpression(ParameterExpression eventParameter) =>
        Expression.Convert(eventParameter, Message.DeclaredType);
}
