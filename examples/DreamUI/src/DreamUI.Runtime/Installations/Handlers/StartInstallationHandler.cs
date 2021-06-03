using System;
using System.Threading;
using System.Threading.Tasks;
using DreamUI.Installations.Timelines;
using Totem;

namespace DreamUI.Installations.Handlers
{
    public class StartInstallationHandler : ILocalCommandHandler<StartInstallation>
    {
        readonly ITimelineRepository<InstallationTimeline> _repository;

        public StartInstallationHandler(ITimelineRepository<InstallationTimeline> repository) =>
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        public async Task HandleAsync(ILocalCommandContext<StartInstallation> context, CancellationToken cancellationToken)
        {
            var timeline = await _repository.LoadAsync(InstallationTimeline.TimelineId, cancellationToken);

            timeline.When(context.Command);

            if(timeline.HasErrors)
            {
                context.AddErrors(timeline.Errors);
                return;
            }

            await _repository.SaveAsync(timeline, context.CorrelationId, context.Principal, cancellationToken);
        }
    }
}