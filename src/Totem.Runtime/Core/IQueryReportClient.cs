namespace Totem.Core;

public interface IQueryReportClient
{
    Task LoadResultAsync(IQueryContext<IQueryMessage> context, CancellationToken cancellationToken);
}
