namespace Totem.Map.Summary;

public class HttpRequestSummary
{
    internal HttpRequestSummary(string href, string method, string route)
    {
        Href = href;
        Method = method;
        Route = route;
    }

    public string Href { get; }
    public string Method { get; }
    public string Route { get; }
}
