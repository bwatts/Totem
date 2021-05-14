namespace Totem.Core
{
    public interface IQueueEnvelope : IMessageEnvelope
    {
        new IQueueCommand Message { get; }
        Text QueueName { get; }
    }
}