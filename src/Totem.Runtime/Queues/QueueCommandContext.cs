using Totem.Core;
using Totem.Map;

namespace Totem.Queues;

public class QueueCommandContext<TCommand> : CommandContext<TCommand>, IQueueCommandContext<TCommand>
    where TCommand : IQueueCommand
{
    internal QueueCommandContext(Id pipelineId, IQueueCommandEnvelope envelope, CommandType commandType) : base(pipelineId, envelope, commandType)
    { }

    public new IQueueCommandEnvelope Envelope => (IQueueCommandEnvelope) base.Envelope;
    public new QueueCommandInfo Info => (QueueCommandInfo) base.Info;
    public override Type InterfaceType => typeof(IQueueCommandContext<TCommand>);
    public Text QueueName => Info.QueueName;
}
