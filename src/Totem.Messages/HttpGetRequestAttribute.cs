using Totem.Http;

namespace Totem;

public class HttpGetRequestAttribute : HttpRequestAttribute
{
    public HttpGetRequestAttribute(string route) : base(HttpMethod.Get.ToString(), route)
    { }
}
