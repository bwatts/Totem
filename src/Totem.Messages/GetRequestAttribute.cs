using System.Net.Http;
using Totem.Http;

namespace Totem
{
    public class GetRequestAttribute : HttpRequestAttribute
    {
        public GetRequestAttribute(string route) : base(HttpMethod.Get.ToString(), route)
        { }
    }
}