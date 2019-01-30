using System;
using System.Threading.Tasks;
using Totem.Runtime;
using Totem.Timeline.Area;

namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// The scope of a query's activity on the timeline
  /// </summary>
  public class QueryScope : FlowScope<Query>
  {
    readonly int _batchSize;
    int _batchCount;

    public QueryScope(FlowKey key, ITimelineDb db) : base(key, db)
    {
      _batchSize = ((QueryType) key.Type).BatchSize;
    }

    protected override async Task ObservePoint()
    {
      try
      {
        CallGiven();
      }
      catch
      {
        await CompleteBatch();

        throw;
      }

      await AdvanceBatch();
    }

    protected override Task OnPendingDequeue() =>
      CompleteBatch();

    void CallGiven()
    {
      Log.Debug("[timeline] #{Position} => {Key}", Point.Position.ToInt64(), Key);

      Flow.Context.CallGiven(new FlowCall.Given(Point, Observation), advanceCheckpoint: true);

      Flow.OnChanged();
    }

    async Task AdvanceBatch()
    {
      if(_batchCount < _batchSize)
      {
        _batchCount++;
      }
      else
      {
        await CompleteBatch();
      }
    }

    async Task CompleteBatch()
    {
      if(_batchCount == 0)
      {
        return;
      }

      try
      {
        await WriteCheckpoint();

        if(_batchCount > 1)
        {
          Log.Debug("[timeline] Wrote {Key} to timeline after batch of {BatchCount}", Key, _batchCount);
        }

        _batchCount = 0;
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] Failed to write {Key} to timeline after batch of {BatchCount}", Key, _batchCount);

        CompleteTask(error);
      }
    }
  }
}