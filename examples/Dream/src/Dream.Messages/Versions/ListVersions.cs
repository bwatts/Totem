using Totem;

namespace Dream.Versions
{
    [GetRequest("/versions")]
    public class ListVersions : IHttpQuery<VersionRecord[]>
    {

    }
}