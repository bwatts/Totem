namespace Totem.Map.Summary;

public class HttpRequestSummary
{
    internal HttpRequestSummary(string method, string route)
    {
        Method = method;
        Route = route;
    }

    public string Method { get; }
    public string Route { get; }
}
