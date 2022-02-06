using System.Threading;
using System.Threading.Tasks;
using Totem;
using Totem.Map;
using Totem.Map.Summary;

namespace Dream;

public class GetMapSummaryHandler : IHttpQueryHandler<GetMapSummary>, ILocalQueryHandler<GetMapSummary>
{
    readonly RuntimeMap _map;

    public GetMapSummaryHandler(RuntimeMap map) =>
        _map = map;

    public Task HandleAsync(IHttpQueryContext<GetMapSummary> context, CancellationToken cancellationToken)
    {
        context.Result = _map.Summarize();

        return Task.CompletedTask;
    }

    public Task HandleAsync(ILocalQueryContext<GetMapSummary> context, CancellationToken cancellationToken)
    {
        context.Result = _map.Summarize();

        return Task.CompletedTask;
    }
}
