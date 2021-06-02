using System.Net.Http;
using Totem.Http;

namespace Totem
{
    public class PostRequestAttribute : HttpRequestAttribute
    {
        public PostRequestAttribute(string route) : base(HttpMethod.Post.ToString(), route)
        { }
    }
}