using System;
using System.Threading;
using System.Threading.Tasks;
using Dream.Versions.Timelines;
using Totem;

namespace Dream.Versions.Handlers
{
    public class DownloadVersionHandler : IQueueHandler<DownloadVersion>
    {
        readonly ITimelineRepository<DownloadTimeline> _repository;
        readonly IVersionService _service;

        public DownloadVersionHandler(ITimelineRepository<DownloadTimeline> repository, IVersionService service)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task HandleAsync(IQueueContext<DownloadVersion> context, CancellationToken cancellationToken)
        {
            var timelineId = DownloadTimeline.DeriveId(context.Command.VersionId);
            var timeline = await _repository.LoadAsync(timelineId, cancellationToken);

            await timeline.WhenAsync(context.Command, _service, cancellationToken);

            if(timeline.HasErrors)
            {
                context.AddErrors(timeline.Errors);
                return;
            }

            await _repository.SaveAsync(timeline, context.CorrelationId, context.Principal, cancellationToken);
        }
    }
}