using System.Net.Http;
using Totem.Http;

namespace Totem
{
    public class HeadRequestAttribute : HttpRequestAttribute
    {
        public HeadRequestAttribute(string route) : base(HttpMethod.Head.ToString(), route)
        { }
    }
}