using System.Net.Http;
using Totem.Http;

namespace Totem
{
    public class DeleteRequestAttribute : HttpRequestAttribute
    {
        public DeleteRequestAttribute(string route) : base(HttpMethod.Delete.ToString(), route)
        { }
    }
}