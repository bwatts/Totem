using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Totem
{
    public interface ITimelineRepository<TTimeline> where TTimeline : ITimeline
    {
        Task<TTimeline> LoadAsync(Id timelineId, CancellationToken cancellationToken);
        Task SaveAsync(ITimeline timeline, Id correlationId, ClaimsPrincipal principal, CancellationToken cancellationToken);
    }
}