using System;

namespace Totem.Map.Summary;

public static partial class MapSummaryExtensions
{
    public static MapSummary Summarize(this RuntimeMap map)
    {
        if(map is null)
            throw new ArgumentNullException(nameof(map));

        return new MapSummaryBuilder(map).Build();
    }
}
