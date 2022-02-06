using Totem.Core;

namespace Totem.Map;

public class TopicCommandContext : ITypeKeyed
{
    internal TopicCommandContext(Type contextType, CommandInfo info)
    {
        ContextType = contextType;
        Info = info;
    }

    public Type ContextType { get; }
    public CommandInfo Info { get; }
    public TopicRouteMethod? Route { get; internal set; }
    public TopicWhenMethod? When { get; internal set; }

    Type ITypeKeyed.TypeKey => ContextType;

    public override string ToString() =>
        ContextType.ToString();
}
