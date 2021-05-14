using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Events;

namespace Totem.Routes
{
    public interface IRouteSubscription : IDisposable
    {
        Task EnqueueAsync(IEventEnvelope envelope, CancellationToken cancellationToken);
    }
}