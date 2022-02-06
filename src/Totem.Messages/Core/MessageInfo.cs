namespace Totem.Core;

public abstract class MessageInfo
{
    internal MessageInfo(Type declaredType) =>
        DeclaredType = declaredType;

    public Type DeclaredType { get; }

    public override string ToString() =>
        DeclaredType.ToString();
}
