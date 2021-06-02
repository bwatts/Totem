using System;
using System.Security.Claims;

namespace Totem.Core
{
    public class EventEnvelope : MessageEnvelope, IEventEnvelope
    {
        public EventEnvelope(
            Id messageId,
            IEvent message,
            EventInfo info,
            Id correlationId,
            ClaimsPrincipal principal,
            Type timelineType,
            Id timelineId,
            long timelineVersion,
            DateTimeOffset whenOccurred) : base(messageId, message, info, correlationId, principal)
        {
            TimelineType = timelineType ?? throw new ArgumentNullException(nameof(timelineType));
            TimelineId = timelineId ?? throw new ArgumentNullException(nameof(timelineId));
            TimelineVersion = timelineVersion;
            WhenOccurred = whenOccurred;
        }

        public new IEvent Message => (IEvent) base.Message;
        public new EventInfo Info => (EventInfo) base.Info;
        public Type TimelineType { get; }
        public Id TimelineId { get; }
        public long TimelineVersion { get; }
        public DateTimeOffset WhenOccurred { get; }

        public override string ToString() =>
            $"{TimelineType.Name}.{TimelineId.ToShortString()}@{TimelineVersion} => {Info.MessageType.Name}.{MessageId.ToShortString()}";
    }
}