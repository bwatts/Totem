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

      await AdvanceBatch();
    }

    void CallGiven()
    {
      Log.Trace("[timeline] #{Position} => {Key}", Point.Position.ToInt64(), Key);

      Flow.Context.CallGiven(new FlowCall.Given(Point, Observation), advanceCheckpoint: true);

      Flow.OnChanged();
    }

    async Task AdvanceBatch()
    {
      if(_batchCount < _batchSize && HasPointEnqueued)
      {
        _batchCount++;
      }
      else
      {
        _batchCount = 0;

        await WriteCheckpoint();
      }
    }
  }
}