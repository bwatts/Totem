using System;

namespace Totem.Map;

public abstract class MessageType : MapType
{
    internal MessageType(Type declaredType) : base(declaredType)
    { }
}
