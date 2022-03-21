using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Map;

public class TopicRouteParameter : TimelineMethodParameter
{
    internal TopicRouteParameter(ParameterInfo info, CommandType message) : base(info, message)
    { }

    public new CommandType Message => (CommandType) base.Message;

    internal Expression ToExpression(ParameterExpression commandParameter) =>
        Expression.Convert(commandParameter, Message.DeclaredType);
}
