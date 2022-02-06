using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Map;

public class ObserverMethodParameter : TimelineMethodParameter
{
    internal ObserverMethodParameter(ParameterInfo info, EventType message, bool hasContext) : base(info, message) =>
        HasContext = hasContext;

    public new EventType Message => (EventType) base.Message;
    public bool HasContext { get; }

    internal Expression ToArgument(ParameterExpression eventContext)
    {
        if(HasContext)
        {
            return eventContext;
        }

        var getEvent = Expression.Property(eventContext, nameof(IEventContext<IEvent>.Event));

        return Expression.Convert(getEvent, Message.DeclaredType);
    }
}
