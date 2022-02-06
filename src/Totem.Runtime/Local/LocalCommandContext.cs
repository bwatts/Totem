using System;
using Totem.Core;
using Totem.Map;

namespace Totem.Local;

public class LocalCommandContext<TCommand> : CommandContext<TCommand>, ILocalCommandContext<TCommand>
    where TCommand : ILocalCommand
{
    internal LocalCommandContext(Id pipelineId, ILocalCommandEnvelope envelope, CommandType commandType) : base(pipelineId, envelope, commandType)
    { }

    public new ILocalCommandEnvelope Envelope => (ILocalCommandEnvelope) base.Envelope;
    public new LocalCommandInfo Info => (LocalCommandInfo) base.Info;
    public override Type InterfaceType => typeof(ILocalCommandContext<TCommand>);
}
