using Totem.Core;

namespace Totem.Map;

public class TopicWhenContext
{
    internal TopicWhenContext(Type interfaceType, CommandInfo info, TopicWhenMethod when)
    {
        InterfaceType = interfaceType;
        Info = info;
        When = when;
    }

    public Type InterfaceType { get; }
    public CommandInfo Info { get; }
    public TopicWhenMethod When { get; }

    internal bool TryGetWhen(ICommandContext<ICommandMessage> context, [NotNullWhen(true)] out TopicWhenMethod? when)
    {
        if(context.InterfaceType == InterfaceType)
        {
            when = When;
            return true;
        }

        when = null;
        return false;
    }
}
