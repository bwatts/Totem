using Totem.Http;

namespace Totem;

public class HttpPostRequestAttribute : HttpRequestAttribute
{
    public HttpPostRequestAttribute(string route) : base(HttpMethod.Post.ToString(), route)
    { }
}
