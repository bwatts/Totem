using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Totem.Core;

namespace Totem
{
    public interface ITimelineStore
    {
        IAsyncEnumerable<IEventEnvelope> ReadTimelineAsync(Id timelineId, CancellationToken cancellationToken);
        IAsyncEnumerable<IEventEnvelope> SaveAsync(ITimeline timeline, Id correlationId, ClaimsPrincipal principal, CancellationToken cancellationToken);
    }
}