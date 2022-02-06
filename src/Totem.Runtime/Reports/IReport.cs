using Totem.Core;

namespace Totem.Reports;

public interface IReport : IObserverTimeline
{
    Type RowType { get; }
    IReportRow Row { get; }
}

public interface IReport<TRow> : IReport
    where TRow : IReportRow, new()
{
    new TRow Row { get; }
}
