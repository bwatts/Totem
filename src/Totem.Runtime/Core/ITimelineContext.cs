namespace Totem.Core;

public interface ITimelineContext
{
    IMessageContext MessageContext { get; }
    ItemKey TimelineKey { get; }
}
