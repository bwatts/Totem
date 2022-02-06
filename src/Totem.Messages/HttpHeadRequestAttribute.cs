using System.Net.Http;
using Totem.Http;

namespace Totem;

public class HttpHeadRequestAttribute : HttpRequestAttribute
{
    public HttpHeadRequestAttribute(string route) : base(HttpMethod.Head.ToString(), route)
    { }
}
