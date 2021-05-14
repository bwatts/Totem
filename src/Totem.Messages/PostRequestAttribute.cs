using System.Net.Http;

namespace Totem
{
    public class PostRequestAttribute : CommandAttribute
    {
        public PostRequestAttribute(string route) : base(HttpMethod.Post.ToString(), route)
        { }
    }
}