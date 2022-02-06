using System.Threading;
using System.Threading.Tasks;

namespace Totem.Local;

public interface ILocalCommandPipeline
{
    Id Id { get; }

    Task<ILocalCommandContext<ILocalCommand>> RunAsync(ILocalCommandEnvelope envelope, CancellationToken token);
}
