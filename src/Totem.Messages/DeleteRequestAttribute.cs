using System.Net.Http;

namespace Totem
{
    public class DeleteRequestAttribute : CommandAttribute
    {
        public DeleteRequestAttribute(string route) : base(HttpMethod.Delete.ToString(), route)
        { }
    }
}