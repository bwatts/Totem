using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Totem.Core;
using Totem.Map;

namespace Totem.Topics;

public class TopicContextFactory : ITopicContextFactory
{
    delegate ITopicContext<ICommandMessage> CommandTypeFactory(Id pipelineId, ICommandContext<ICommandMessage> commandContext, ItemKey topicKey);

    readonly ConcurrentDictionary<CommandType, CommandTypeFactory> _factoriesByCommandType = new();
    readonly RuntimeMap _map;

    public TopicContextFactory(RuntimeMap map) =>
        _map = map ?? throw new ArgumentNullException(nameof(map));

    public ITopicContext<ICommandMessage> Create(Id pipelineId, ICommandContext<ICommandMessage> commandContext, ItemKey topicKey)
    {
        if(commandContext is null)
            throw new ArgumentNullException(nameof(commandContext));

        if(topicKey is null)
            throw new ArgumentNullException(nameof(topicKey));

        var topicDeclaredType = topicKey.DeclaredType;

        var factory = _factoriesByCommandType.GetOrAdd(
            commandContext.CommandType,
            commandType => CompileFactory(topicDeclaredType, commandType));

        return factory(pipelineId, commandContext, topicKey);
    }

    CommandTypeFactory CompileFactory(Type topicDeclaredType, CommandType commandType)
    {
        if(!_map.Topics.TryGet(topicDeclaredType, out var topicType))
            throw new ArgumentException($"Expected known topic of type {topicDeclaredType}", nameof(topicDeclaredType));

        // (pipelineId, commandContext, topicKey) =>
        //      new TopicContext<TCommand>(pipelineId, (ICommandContext<TCommand>) commandContext, topicKey, topicType)

        var pipelineIdParameter = Expression.Parameter(typeof(Id), "pipelineId");
        var commandContextParameter = Expression.Parameter(typeof(ICommandContext<ICommandMessage>), "commandContext");
        var commandContextCast = Expression.Convert(commandContextParameter, typeof(ICommandContext<>).MakeGenericType(commandType.DeclaredType));
        var topicKeyParameter = Expression.Parameter(typeof(ItemKey), "topicKey");
        var constructor = typeof(TopicContext<>)
            .MakeGenericType(commandType.DeclaredType)
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .Single();

        var lambda = Expression.Lambda<CommandTypeFactory>(
            Expression.New(constructor, pipelineIdParameter, commandContextCast, topicKeyParameter, Expression.Constant(topicType)),
            pipelineIdParameter,
            commandContextParameter,
            topicKeyParameter);

        return lambda.Compile();
    }
}
