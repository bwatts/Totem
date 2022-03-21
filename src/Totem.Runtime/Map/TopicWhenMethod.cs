using System.Linq.Expressions;
using System.Reflection;
using Totem.Core;
using Totem.Topics;

namespace Totem.Map;

public class TopicWhenMethod : TimelineMethod
{
    readonly Func<ICommandContext<ICommandMessage>, ITopic, CancellationToken, Task>? _callAsync;
    readonly Action<ICommandContext<ICommandMessage>, ITopic>? _call;

    internal TopicWhenMethod(MethodInfo info, TopicWhenParameter parameter, bool isAsync, bool hasCancellationToken) : base(info, parameter)
    {
        IsAsync = isAsync;
        HasCancellationToken = hasCancellationToken;

        // (context, topic) => ((TTopic) topic).info(<parameter(context)>[, cancellationToken])

        var contextParameter = Expression.Parameter(typeof(ICommandContext<ICommandMessage>), "context");
        var topicParameter = Expression.Parameter(typeof(ITopic), "topic");
        var topicCast = Expression.Convert(topicParameter, info.DeclaringType!);
        var contextArgument = parameter.ToExpression(contextParameter);
        var cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

        if(isAsync)
        {
            var callWhen = hasCancellationToken
                ? Expression.Call(topicCast, info, contextArgument, cancellationTokenParameter)
                : Expression.Call(topicCast, info, contextArgument);

            var lambda = Expression.Lambda<Func<ICommandContext<ICommandMessage>, ITopic, CancellationToken, Task>>(callWhen, contextParameter, topicParameter, cancellationTokenParameter);

            _callAsync = lambda.Compile();
        }
        else
        {
            var callWhen = Expression.Call(topicCast, info, contextArgument);

            var lambda = Expression.Lambda<Action<ICommandContext<ICommandMessage>, ITopic>>(callWhen, contextParameter, topicParameter);

            _call = lambda.Compile();
        }
    }

    public new TopicWhenParameter Parameter => (TopicWhenParameter) base.Parameter;
    public bool IsAsync { get; }
    public bool HasCancellationToken { get; }

    internal async Task CallAsync(ICommandContext<ICommandMessage> context, ITopic topic, CancellationToken cancellationToken)
    {
        if(_callAsync is not null)
        {
            await _callAsync(context, topic, cancellationToken);
        }
        else
        {
            _call!(context, topic);
        }
    }
}
