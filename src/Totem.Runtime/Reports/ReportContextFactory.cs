using System.Collections.Concurrent;
using Totem.Core;
using Totem.Map;

namespace Totem.Reports;

public class ReportContextFactory : IReportContextFactory
{
    readonly ConcurrentDictionary<Type, ObserverContextFactory<IReportContext<IEvent>>> _factoriesByReportType = new();
    readonly RuntimeMap _map;

    public ReportContextFactory(RuntimeMap map) =>
        _map = map ?? throw new ArgumentNullException(nameof(map));

    public IReportContext<IEvent> Create(Id pipelineId, IEventContext<IEvent> eventContext, ItemKey reportKey)
    {
        if(eventContext is null)
            throw new ArgumentNullException(nameof(eventContext));

        if(reportKey is null)
            throw new ArgumentNullException(nameof(reportKey));

        var factory = _factoriesByReportType.GetOrAdd(
            reportKey.DeclaredType,
            type => new(_map.Reports[reportKey.DeclaredType], typeof(ReportContext<>)));

        return factory.Create(pipelineId, eventContext, reportKey);
    }
}
