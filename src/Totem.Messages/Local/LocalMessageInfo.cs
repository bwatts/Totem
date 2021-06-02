using System;
using Totem.Core;

namespace Totem.Local
{
    public abstract class LocalMessageInfo : MessageInfo
    {
        protected LocalMessageInfo(Type messageType) : base(messageType)
        { }
    }
}