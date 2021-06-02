using System;

namespace Totem.Core
{
    public abstract class MessageInfo
    {
        protected MessageInfo(Type messageType) =>
            MessageType = messageType ?? throw new ArgumentNullException(nameof(messageType));

        public Type MessageType { get; }

        public override string ToString() => MessageType.ToString();
    }
}