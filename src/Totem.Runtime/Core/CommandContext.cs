using System;
using Totem.Map;

namespace Totem.Core;

public abstract class CommandContext<TCommand> : MessageContext, ICommandContext<TCommand>
    where TCommand : ICommandMessage
{
    internal CommandContext(Id pipelineId, ICommandEnvelope envelope, CommandType commandType) : base(pipelineId, envelope) =>
        CommandType = commandType;

    public new ICommandEnvelope Envelope => (ICommandEnvelope) base.Envelope;
    public new CommandInfo Info => (CommandInfo) base.Info;
    public abstract Type InterfaceType { get; }
    public TCommand Command => (TCommand) Envelope.Message;
    public ItemKey CommandKey => Envelope.MessageKey;
    public CommandType CommandType { get; }
    public Id CommandId => Envelope.MessageKey.Id;

    CommandInfo ICommandContext<TCommand>.Info => Info;
    ICommandEnvelope ICommandContext<TCommand>.Envelope => Envelope;
}
