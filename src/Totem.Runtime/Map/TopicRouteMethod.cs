using System.Linq.Expressions;
using System.Reflection;
using Totem.Core;

namespace Totem.Map;

public class TopicRouteMethod : TopicMethod
{
    readonly Func<ICommandContext<ICommandMessage>, Id> _call;

    internal TopicRouteMethod(MethodInfo info, TopicMethodParameter parameter) : base(info, parameter)
    {
        // context => info(<parameter>)

        var contextParameter = Expression.Parameter(typeof(ICommandContext<ICommandMessage>), "context");
        var call = Expression.Call(info, parameter.ToArgument(contextParameter));
        var lambda = Expression.Lambda<Func<ICommandContext<ICommandMessage>, Id>>(call, contextParameter);

        _call = lambda.Compile();
    }

    internal ItemKey Call(ICommandContext<ICommandMessage> context) =>
        new(Info.DeclaringType!, _call(context));
}
