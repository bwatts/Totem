using System;
using System.Security.Claims;

namespace Totem.Core
{
    public class EventEnvelope : MessageEnvelope, IEventEnvelope
    {
        public EventEnvelope(
            IEvent e,
            Id eventId,
            Id correlationId,
            ClaimsPrincipal principal,
            Type timelineType,
            Id timelineId,
            long timelineVersion,
            DateTimeOffset whenOccurred)
            : base(e, eventId, correlationId, principal)
        {
            Message = e;
            TimelineType = timelineType ?? throw new ArgumentNullException(nameof(timelineType));
            TimelineId = timelineId ?? throw new ArgumentNullException(nameof(timelineId));
            TimelineVersion = timelineVersion;
            WhenOccurred = whenOccurred;
        }

        public new IEvent Message { get; }
        public Type TimelineType { get; }
        public Id TimelineId { get; }
        public long TimelineVersion { get; }
        public DateTimeOffset WhenOccurred { get; }

        public override string ToString() =>
            $"{TimelineType.Name}.{TimelineId.ToCompactString()}@{TimelineVersion} => {MessageType.Name}.{MessageId.ToCompactString()}";
    }
}