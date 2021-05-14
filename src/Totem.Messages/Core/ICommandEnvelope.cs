namespace Totem.Core
{
    public interface ICommandEnvelope : IMessageEnvelope
    {
        new ICommand Message { get; }
    }
}