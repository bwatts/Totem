using System.Linq.Expressions;
using System.Reflection;
using Totem.Core;

namespace Totem.Map;

public class TopicWhenParameter : TimelineMethodParameter
{
    internal TopicWhenParameter(ParameterInfo info, CommandType message, Type? contextType) : base(info, message) =>
        ContextType = contextType;

    public new CommandType Message => (CommandType) base.Message;
    public Type? ContextType { get; }

    internal Expression ToExpression(ParameterExpression context)
    {
        if(ContextType is not null)
        {
            // (TContext) context

            return Expression.Convert(context, ContextType);
        }

        // (TCommand) context.Command

        var getCommand = Expression.Property(context, nameof(ICommandContext<ICommandMessage>.Command));

        return Expression.Convert(getCommand, Info.ParameterType);
    }
}
