using Totem.Core;
using Totem.Topics;

namespace Totem.Map;

public class TopicType : TimelineType
{
    internal TopicType(Type declaredType) : base(declaredType)
    { }

    public Id? SingleInstanceId { get; internal set; }
    public TypeKeyedCollection<CommandType> Commands { get; } = new();
    public TypeKeyedCollection<GivenMethod> Givens { get; } = new();

    internal ItemKey CallSingleInstanceRoute()
    {
        if(SingleInstanceId is null)
            throw new InvalidOperationException($"Topic {this} is not single-instance");

        return new(DeclaredType, SingleInstanceId);
    }

    internal void CallGivenIfDefined(ITopic topic, IEvent e)
    {
        if(Givens.TryGet(e.GetType(), out var given))
        {
            given.Call(topic, e);
        }
    }
}
