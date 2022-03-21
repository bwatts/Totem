using System.Linq.Expressions;
using System.Reflection;
using Totem.Core;

namespace Totem.Map;

public class TopicRouteMethod : TimelineMethod
{
    readonly Func<ICommandMessage, Id> _call;

    internal TopicRouteMethod(MethodInfo info, TopicRouteParameter parameter) : base(info, parameter)
    {
        // command => info(<parameter>)

        var commandParameter = Expression.Parameter(typeof(ICommandMessage), "command");
        var call = Expression.Call(info, parameter.ToExpression(commandParameter));
        var lambda = Expression.Lambda<Func<ICommandMessage, Id>>(call, commandParameter);

        _call = lambda.Compile();
    }

    public new TopicRouteParameter Parameter => (TopicRouteParameter) base.Parameter;

    internal ItemKey Call(ICommandMessage command) =>
        new(Info.DeclaringType!, _call(command));
}
