using Totem.Core;
using Totem.Map;

namespace Totem.Reports;

public interface IReportContext<out TEvent> where TEvent : IEvent
{
    Id PipelineId { get; }
    IEventContext<TEvent> EventContext { get; }
    ItemKey ReportKey { get; }
    ReportType ReportType { get; }
}
