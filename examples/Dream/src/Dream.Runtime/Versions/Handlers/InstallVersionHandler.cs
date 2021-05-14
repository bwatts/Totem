using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dream.Versions.Timelines;
using Totem;

namespace Dream.Versions.Handlers
{
    public class InstallVersionHandler : ICommandHandler<InstallVersion>
    {
        readonly ITimelineRepository<VersionsTimeline> _repository;

        public InstallVersionHandler(ITimelineRepository<VersionsTimeline> repository) =>
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        public async Task HandleAsync(ICommandContext<InstallVersion> context, CancellationToken cancellationToken)
        {
            var timeline = await _repository.LoadAsync(VersionsTimeline.TimelineId, cancellationToken);

            timeline.When(context.Command);

            if(timeline.HasErrors)
            {
                context.AddErrors(timeline.Errors);
                return;
            }

            await _repository.SaveAsync(timeline, context.CorrelationId, context.Principal, cancellationToken);

            var installed = timeline.NewEvents.OfType<VersionInstalled>().FirstOrDefault();

            if(installed != null)
            {
                context.RespondCreated($"/versions/{installed.VersionId}");
            }
        }
    }
}