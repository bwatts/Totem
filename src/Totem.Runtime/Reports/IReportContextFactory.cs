using Totem.Core;

namespace Totem.Reports;

public interface IReportContextFactory
{
    IReportContext<IEvent> Create(Id pipelineId, IEventContext<IEvent> eventContext, ItemKey reportKey);
}
