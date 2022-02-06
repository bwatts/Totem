using Totem.Map.Summary;

namespace Dream;

[HttpGetRequest("/api/map")]
public class GetMapSummary : IHttpQuery<MapSummary>, ILocalQuery<MapSummary>
{

}
