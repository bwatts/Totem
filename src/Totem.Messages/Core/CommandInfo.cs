namespace Totem.Core;

public abstract class CommandInfo : MessageInfo
{
    internal CommandInfo(Type declaredType) : base(declaredType)
    { }
}
