using Totem.Map;

namespace Totem.Reports.Subscriptions;

public class ReportVersion
{
    internal ReportVersion(ReportType report, Id routeId, int number)
    {
        if(report is null)
            throw new ArgumentNullException(nameof(report));

        if(routeId is null)
            throw new ArgumentNullException(nameof(routeId));

        if(number < 0)
            throw new ArgumentOutOfRangeException(nameof(number));

        Report = report;
        RouteId = routeId;
        Number = number;
    }

    public ReportType Report { get; }
    public Id RouteId { get; }
    public int Number { get; }

    public override string ToString() =>
        $"{Report.DeclaredType.FullName}/{RouteId}@{Number}";

    public ReportETag ToListETag() =>
        ReportETag.List(Report, RouteId, Number);

    public ReportETag ToRouteETag() =>
        ReportETag.Route(Report, RouteId, Number);
}
