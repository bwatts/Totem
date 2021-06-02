using System;
using Totem.Core;
using Totem.Local;

namespace Totem
{
    public interface ILocalCommandContext<out TCommand> : IMessageContext
        where TCommand : ILocalCommand
    {
        new ILocalCommandEnvelope Envelope { get; }
        new LocalCommandInfo Info { get; }
        TCommand Command { get; }
        Type CommandType { get; }
        Id CommandId { get; }
    }
}