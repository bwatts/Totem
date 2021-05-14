using System.Net.Http;

namespace Totem
{
    public class HeadRequestAttribute : QueryAttribute
    {
        public HeadRequestAttribute(string route) : base(HttpMethod.Head.ToString(), route)
        { }
    }
}