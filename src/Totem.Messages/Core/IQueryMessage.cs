namespace Totem.Core
{
    public interface IQueryMessage : IMessage
    {

    }

    public interface IQueryMessage<TResult> : IQueryMessage
    {

    }
}