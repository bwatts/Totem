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
      CallGiven();

      if(_batchCount < _batchSize)
      {
        _batchCount++;
      }
      else
      {
        await CompleteBatch();
      }
    }

    protected override Task OnPendingDequeue() =>
      CompleteBatch();

    void CallGiven()
    {
      Log.Debug("[timeline] #{Position} => {Key}", Point.Position.ToInt64(), Key);

      Flow.Context.CallGiven(new FlowCall.Given(Point, Observation), advanceCheckpoint: true);

      Flow.OnChanged();
    }

    async Task CompleteBatch()
    {
      if(_batchCount > 0)
      {
        await WriteCheckpoint();

        if(_batchCount > 1)
        {
          Log.Debug("[timeline] Wrote {Key} to timeline after batch of {BatchCount}", Key, _batchCount);
        }

        _batchCount = 0;
      }
    }
  }
}