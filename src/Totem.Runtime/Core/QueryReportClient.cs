namespace Totem.Core;

public class QueryReportClient : IQueryReportClient
{
    readonly IStorage _storage;

    public QueryReportClient(IStorage storage) =>
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));

    public async Task LoadResultAsync(IQueryContext<IQueryMessage> context, CancellationToken cancellationToken)
    {
        var result = context.Info.Result;

        if(result.RowType is null)
        {
            // TODO: Error
            return;
        }

        var partitionKey = result.RowType.FullName!;
        var rowKey = context.QueryId.ToShortString();





        // TODO: This requires the result type to be IReadOnlyList<TRow> ?



        // Need to determine how to do this in the abstract










        if(!result.IsSingleRow)
        {
            context.Result = await _storage.ListAsync(partitionKey, cancellationToken);
            return;
        }

        var key = new StorageKey(partitionKey, rowKey);
        var row = await _storage.GetAsync(key, cancellationToken);

        if(row is null)
        {
            context.AddError(RuntimeErrors.ReportNotFound);
        }
        else
        {
            context.Result = row.Value;
        }
    }
}
