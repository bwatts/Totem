using System.Threading;
using System.Threading.Tasks;
using Totem.Core;

namespace Totem.Commands
{
    public interface ICommandPipeline
    {
        Id Id { get; }

        Task<ICommandContext<ICommand>> RunAsync(ICommandEnvelope envelope, CancellationToken token);
    }
}