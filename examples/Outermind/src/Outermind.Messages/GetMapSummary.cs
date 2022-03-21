using Totem.Map.Summary;

namespace Outermind;

[HttpGetRequest("/api/map")]
public class GetMapSummary : IHttpQuery<MapSummary>, ILocalQuery<MapSummary>
{

}
