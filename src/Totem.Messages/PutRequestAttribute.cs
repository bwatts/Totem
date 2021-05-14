using System.Net.Http;

namespace Totem
{
    public class PutRequestAttribute : CommandAttribute
    {
        public PutRequestAttribute(string route) : base(HttpMethod.Put.ToString(), route)
        { }
    }
}