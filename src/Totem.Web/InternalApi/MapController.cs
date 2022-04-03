using Microsoft.AspNetCore.Mvc;
using Totem.Map;
using Totem.Map.Summary;

namespace Totem.InternalApi;

[Route("map")]
internal class MapController : ControllerBase
{
    readonly RuntimeMap _map;

    public MapController(RuntimeMap map) =>
        _map = map ?? throw new ArgumentNullException(nameof(map));

    [HttpGet]
    public MapSummary GetMap() =>
        _map.Summarize();
}
