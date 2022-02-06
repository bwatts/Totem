using Totem.Core;
using Totem.Map;

namespace Totem.Reports;

public class ReportContext<TEvent> : MessageContext, IReportContext<TEvent>
    where TEvent : class, IEvent
{
    internal ReportContext(Id pipelineId, IEventContext<TEvent> eventContext, ItemKey reportKey, ReportType reportType)
        : base(pipelineId, eventContext.Envelope)
    {
        EventContext = eventContext;
        ReportKey = reportKey;
        ReportType = reportType;
    }

    public IEventContext<TEvent> EventContext { get; }
    public ItemKey ReportKey { get; }
    public ReportType ReportType { get; }
}
