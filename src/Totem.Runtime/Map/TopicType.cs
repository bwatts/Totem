using Totem.Topics;

namespace Totem.Map;

public class TopicType : MapType
{
    internal TopicType(Type declaredType) : base(declaredType)
    { }

    public TypeKeyedCollection<CommandType> Commands { get; } = new();
    public TypeKeyedCollection<GivenMethod> Givens { get; } = new();

    internal void CallGivenIfDefined(ITopic topic, IEvent e)
    {
        if(Givens.TryGet(e.GetType(), out var given))
        {
            given.Call(topic, e);
        }
    }
}
