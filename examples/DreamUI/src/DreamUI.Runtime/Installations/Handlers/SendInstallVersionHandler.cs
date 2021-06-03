using System;
using System.Threading;
using System.Threading.Tasks;
using DreamUI.Installations.Timelines;
using Totem;

namespace DreamUI.Installations.Handlers
{
    public class SendInstallVersionHandler : IQueueCommandHandler<SendInstallVersion>
    {
        readonly ITimelineRepository<InstallationTimeline> _repository;
        readonly ITotemClient _client;

        public SendInstallVersionHandler(ITimelineRepository<InstallationTimeline> repository, ITotemClient client)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task HandleAsync(IQueueCommandContext<SendInstallVersion> context, CancellationToken cancellationToken)
        {
            var timeline = await _repository.LoadAsync(InstallationTimeline.TimelineId, cancellationToken);

            await timeline.WhenAsync(context.Command, _client, cancellationToken);

            if(timeline.HasErrors)
            {
                context.AddErrors(timeline.Errors);
                return;
            }

            await _repository.SaveAsync(timeline, context.CorrelationId, context.Principal, cancellationToken);
        }
    }
}