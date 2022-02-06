using Totem.Http;

namespace Totem;

public class HttpDeleteRequestAttribute : HttpRequestAttribute
{
    public HttpDeleteRequestAttribute(string route) : base(HttpMethod.Delete.ToString(), route)
    { }
}
