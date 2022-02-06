using Totem.Local;

namespace Totem;

public interface ILocalClient
{
    Task<ILocalCommandContext<ILocalCommand>> SendAsync(ILocalCommandEnvelope envelope, CancellationToken cancellationToken);
    Task<ILocalQueryContext<ILocalQuery>> SendAsync(ILocalQueryEnvelope envelope, CancellationToken cancellationToken);
}
