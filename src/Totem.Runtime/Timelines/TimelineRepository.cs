using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Totem.Timelines
{
    public class TimelineRepository<TTimeline> : ITimelineRepository<TTimeline>
        where TTimeline : ITimeline
    {
        readonly ILogger _logger;
        readonly ITimelineStore _store;

        public TimelineRepository(ILogger<TimelineRepository<TTimeline>> logger, ITimelineStore store)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<TTimeline> LoadAsync(Id timelineId, CancellationToken cancellationToken)
        {
            if(timelineId == null)
                throw new ArgumentNullException(nameof(timelineId));

            var timeline = _factory(timelineId);

            await foreach(var envelope in _store.ReadTimelineAsync(timelineId, cancellationToken))
            {
                timeline.Load(envelope.Message, envelope.TimelineVersion);
            }

            if(timeline.Version != null)
            {
                _logger.LogTrace("[timeline] Loaded {@TimelineType}.{@TimelineId}@{TimelineVersion}", timeline.GetType(), timelineId, timeline.Version);
            }

            return timeline;
        }

        public async Task SaveAsync(ITimeline timeline, Id correlationId, ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            if(timeline == null)
                throw new ArgumentNullException(nameof(timeline));

            if(timeline.HasErrors)
                throw new ArgumentException($"Timeline is in an error state: {timeline.GetType().Name}.{timeline.Id.ToShortString()}", nameof(timeline));

            if(!timeline.HasNewEvents)
            {
                return;
            }
            
            await foreach(var envelope in _store.SaveAsync(timeline, correlationId, principal, cancellationToken))
            {
                _logger.LogTrace("[timeline] Append {@EventType}.{@EventId} to {@TimelineType}.{@TimelineId}@{TimelineVersion}", envelope.Info.MessageType, envelope.MessageId, envelope.TimelineType, envelope.TimelineId, envelope.TimelineVersion);
            }
        }

        static readonly Func<Id, TTimeline> _factory;

        static TimelineRepository()
        {
            // id => new TTimeline(id)

            var constructorCalls =
                from constructor in typeof(TTimeline).GetTypeInfo().DeclaredConstructors
                where constructor.IsPublic
                let parameters = constructor.GetParameters()
                where parameters.Length == 1
                where parameters[0].ParameterType == typeof(Id)
                let idParameter = Expression.Parameter(typeof(Id), parameters[0].Name)
                select Expression.Lambda<Func<Id, TTimeline>>(
                    Expression.New(constructor, idParameter),
                    idParameter);

            var constructorCall = constructorCalls.FirstOrDefault();

            if(constructorCall == null)
                throw new Exception($"Timeline {typeof(TTimeline)} must declare a public constructor with a single parameter of type {typeof(Id)}");

            _factory = constructorCall.Compile();
        }
    }
}