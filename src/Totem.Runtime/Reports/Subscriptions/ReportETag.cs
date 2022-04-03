using Totem.Map;

namespace Totem.Reports.Subscriptions;

public class ReportETag
{
    const string _prefix = "totem:";
    const string _listType = "list:";
    const string _routeType = "route:";

    ReportETag(ReportType report, Id routeId, int version, bool isList)
    {
        Report = report ?? throw new ArgumentNullException(nameof(report));
        RouteId = routeId ?? throw new ArgumentNullException(nameof(routeId));
        Version = version >= 0 ? version : throw new ArgumentOutOfRangeException(nameof(version));
        IsList = isList;
    }

    public ReportType Report { get; }
    public Id RouteId { get; }
    public int Version { get; }
    public bool IsList { get; }

    public override string ToString() =>
        $"{_prefix}{(IsList ? _listType : _routeType)}{Report.DeclaredType.FullName}/{RouteId}@{Version}";

    public static ReportETag List(ReportType report, Id latestRouteId, int latestRouteVersion) =>
        new(report, latestRouteId, latestRouteVersion, isList: true);

    public static ReportETag Route(ReportType report, Id id, int version) =>
        new(report, id, version, isList: false);

    public static bool TryFrom(string value, RuntimeMap map, [NotNullWhen(true)] out ReportETag? etag)
    {
        if(value is null)
            throw new ArgumentNullException(nameof(value));

        if(map is null)
            throw new ArgumentNullException(nameof(map));

        etag = null;

        if(!value.StartsWith(_prefix))
        {
            return false;
        }

        var typeValue = value[_prefix.Length..];
        bool isList;
        string reportValue;

        if(typeValue.StartsWith(_listType))
        {
            isList = true;
            reportValue = typeValue[_listType.Length..];
        }
        else if(typeValue.StartsWith(_routeType))
        {
            isList = false;
            reportValue = typeValue[_routeType.Length..];
        }
        else
        {
            return false;
        }

        var reportParts = reportValue.Split('/');

        if(reportParts.Length != 2)
        {
            return false;
        }

        var report = map.Reports.FirstOrDefault(x => x.DeclaredType.FullName == reportParts[0]);

        if(report is null)
        {
            return false;
        }

        var routeParts = reportParts[1].Split('@');

        if(routeParts.Length != 2
            || !Id.TryFrom(routeParts[0], out var routeId)
            || !int.TryParse(routeParts[1], out var version))
        {
            return false;
        }

        etag = new(report, routeId, version, isList);
        return true;
    }

    public static ReportETag From(string value, RuntimeMap map)
    {
        if(!TryFrom(value, map, out var etag))
            throw new ArgumentException($"Expected a report ETag in the format \"totem:<list|route>:<type>/<id>@<version>\": {value}", nameof(value));

        return etag;
    }
}
