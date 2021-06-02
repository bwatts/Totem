using Totem.Core;
using Totem.Http;

namespace Totem
{
    public interface IHttpQuery : IHttpMessage, IQueryMessage
    {

    }

    public interface IHttpQuery<TResult> : IHttpQuery, IQueryMessage<TResult>
    {

    }
}