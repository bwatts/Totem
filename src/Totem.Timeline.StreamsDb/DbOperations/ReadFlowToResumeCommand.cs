using System;
using System.Threading.Tasks;
using StreamsDB.Driver;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.StreamsDb.DbOperations
{
  /// <summary>
  /// Reads the <see cref="FlowResumeInfo"/> for a particular flow
  /// </summary>
  internal sealed class ReadFlowToResumeCommand
  {
    readonly Many<TimelinePoint> _points = new Many<TimelinePoint>();
    readonly StreamsDbContext _context;
    readonly FlowKey _key;
    readonly string _routesStream;
    long? _areaCheckpoint;
    long _routesCheckpoint;
    int _batchIndex;

    internal ReadFlowToResumeCommand(StreamsDbContext context, FlowKey key)
    {
      _context = context;
      _key = key;

      _routesStream = key.GetRoutesStream();
    }

    internal async Task<FlowResumeInfo> Execute()
    {
      var flow = await ReadFlow();

      switch(flow)
      {
        case FlowInfo.NotFound notFound:
          return await StartAndResume();
        case FlowInfo.Loaded loaded:
          return await Resume(loaded.Flow);
        case FlowInfo.Stopped stopped:
          throw new Exception($"Flow is stopped at {stopped.Position} with this error: {stopped.Error}");
        default:
          throw new NotSupportedException($"Unknown flow info type {flow.GetType()}");
      }
    }

    Task<FlowInfo> ReadFlow() =>
      new ReadFlowCommand(_context, _key).Execute();

    Task<FlowResumeInfo> StartAndResume()
    {
      var flow = _key.Type.New();

      FlowContext.Bind(flow, _key);

      return Resume(flow);
    }

    async Task<FlowResumeInfo> Resume(Flow flow)
    {
      _areaCheckpoint = flow.Context.CheckpointPosition.ToInt64OrNull();

      await ReadPoints();

      if(_points.Count == 0)
      {
        throw new Exception($"Flow was specified to resume but has no pending routes");
      }

      return new FlowResumeInfo(flow, _points);
    }

    async Task ReadPoints()
    {
      var (message, found) = await ReadLastRoute();

      if(found)
      {
        await ReadPoints(message);
      }
    }

    Task<(Message, bool)> ReadLastRoute() =>
      _context.Client.DB().ReadLastMessageFromStream($"{_context.AreaName}-{_routesStream}");

    async Task ReadPoints(Message lastRoute)
    {
      if(_areaCheckpoint == null || _areaCheckpoint < lastRoute.Position + 1)
      {
        AddPoint(lastRoute);

        while(await ReadNextBatch())
        {
          _batchIndex++;
        }
      }
    }

    void AddPoint(Message e)
    {
      _points.Write.Insert(0, _context.ReadAreaPoint(e));

      _routesCheckpoint = e.Position;
    }

    async Task<bool> ReadNextBatch()
    {
      if(_routesCheckpoint == 0)
      {
        return false;
      }

      var batch = await ReadBatch();

      foreach(var e in batch.Messages)
      {
        if(e.Position <= _areaCheckpoint)
        {
          return false;
        }

        AddPoint(e);
      }

      return !batch.HasNext;
    }

    async Task<IStreamSlice> ReadBatch()
    {
      var result = await _context.Client.DB().ReadStreamBackward(
        $"{_context.AreaName}-{_routesStream}",
        _routesCheckpoint - 1,
        _key.Type.ResumeAlgorithm.GetNextBatchSize(_batchIndex));

      return result;
    }
  }
}