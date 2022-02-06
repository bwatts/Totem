using Totem.Http;

namespace Totem;

public class HttpPutRequestAttribute : HttpRequestAttribute
{
    public HttpPutRequestAttribute(string route) : base(HttpMethod.Put.ToString(), route)
    { }
}
