using System.Net.Http;
using Totem.Http;

namespace Totem
{
    public class PutRequestAttribute : HttpRequestAttribute
    {
        public PutRequestAttribute(string route) : base(HttpMethod.Put.ToString(), route)
        { }
    }
}