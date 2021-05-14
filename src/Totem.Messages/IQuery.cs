namespace Totem
{
    public interface IQuery : IMessage
    {

    }

    public interface IQuery<in TResult> : IQuery
    {

    }
}