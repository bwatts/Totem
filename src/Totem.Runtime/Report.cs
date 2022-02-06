using System;
using Totem.Core;
using Totem.Reports;

namespace Totem;

public abstract class Report<TRow> : Timeline, IReport<TRow>
    where TRow : IReportRow, new()
{
    public Type RowType => typeof(TRow);

    protected TRow Row { get; } = new();

    IReportRow IReport.Row => Row;
    TRow IReport<TRow>.Row => Row;
}
