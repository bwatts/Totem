using System.Threading;
using System.Threading.Tasks;
using Totem.Core;

namespace Totem.Commands
{
    public interface IClientCommandPipeline
    {
        Id Id { get; }

        Task<IClientCommandContext<ICommand>> RunAsync(ICommandEnvelope envelope, CancellationToken token);
    }
}