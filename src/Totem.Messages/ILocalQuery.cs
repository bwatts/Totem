using Totem.Core;
using Totem.Local;

namespace Totem;

public interface ILocalQuery : ILocalMessage, IQueryMessage
{

}

public interface ILocalQuery<TResult> : ILocalQuery, IQueryMessage<TResult>
{

}
