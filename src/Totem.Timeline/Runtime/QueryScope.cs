using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Totem.Runtime;
using Totem.Timeline.Area;

namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// The scope of a view's activity on the timeline
  /// </summary>
  public class QueryScope : FlowScope<Query>
  {
    readonly int _batchSize;
    int _batchCount;

    public QueryScope(FlowKey key, IServiceProvider services, ITimelineDb db)
      : base(key, services, db)
    {
      _batchSize = ((QueryType) key.Type).BatchSize;
    }

    protected override async Task ObservePoint()
    {
      try
      {
        await CallWhen();
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

    async Task CallWhen()
    {
      Log.Debug("[timeline] #{Position} => {Key}", Point.Position.ToInt64(), Key);

      using(var scope = Services.CreateScope())
      {
        await Flow.Context.CallWhen(new FlowCall.When(
          Point,
          Observation,
          scope.ServiceProvider,
          State.CancellationToken));
      }

      Flow.OnUpdated();
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