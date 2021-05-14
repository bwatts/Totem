using System.Net.Http;

namespace Totem
{
    public class GetRequestAttribute : QueryAttribute
    {
        public GetRequestAttribute(string route) : base(HttpMethod.Get.ToString(), route)
        { }
    }
}