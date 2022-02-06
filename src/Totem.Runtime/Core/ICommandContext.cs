using System;
using Totem.Map;

namespace Totem.Core;

public interface ICommandContext<out TCommand> : IMessageContext
    where TCommand : ICommandMessage
{
    new CommandInfo Info { get; }
    new ICommandEnvelope Envelope { get; }
    Type InterfaceType { get; }
    TCommand Command { get; }
    ItemKey CommandKey { get; }
    CommandType CommandType { get; }
    Id CommandId { get; }
}
