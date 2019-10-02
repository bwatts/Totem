using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.EventStore.DbOperations
{
  /// <summary>
  /// Reads the <see cref="FlowResumeInfo"/> for a particular flow
  /// </summary>
  internal sealed class ReadFlowToResumeCommand
  {
    readonly Many<TimelinePoint> _points = new Many<TimelinePoint>();
    readonly EventStoreContext _context;
    readonly FlowKey _key;
    readonly string _routesStream;
    long? _areaCheckpoint;
    long _routesCheckpoint;
    int _batchIndex;

    internal ReadFlowToResumeCommand(EventStoreContext context, FlowKey key)
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
      var result = await ReadLastRoute();

      if(result.Status == EventReadStatus.Success)
      {
        await ReadPoints(result.Event.Value);
      }
    }

    Task<EventReadResult> ReadLastRoute() =>
      _context.Connection.ReadEventAsync(_routesStream, StreamPosition.End, resolveLinkTos: true);

    async Task ReadPoints(ResolvedEvent lastRoute)
    {
      if(_areaCheckpoint == null || _areaCheckpoint < lastRoute.Event.EventNumber)
      {
        AddPoint(lastRoute);

        while(await ReadNextBatch())
        {
          _batchIndex++;
        }
      }
    }

    void AddPoint(ResolvedEvent e)
    {
      _points.Write.Insert(0, _context.ReadAreaPoint(e));

      _routesCheckpoint = e.Link.EventNumber;
    }

    async Task<bool> ReadNextBatch()
    {
      if(_routesCheckpoint == 0)
      {
        return false;
      }

      var batch = await ReadBatch();

      foreach(var e in batch.Events)
      {
        if(e.Event.EventNumber <= _areaCheckpoint)
        {
          return false;
        }

        AddPoint(e);
      }

      return !batch.IsEndOfStream;
    }

    async Task<StreamEventsSlice> ReadBatch()
    {
      var result = await _context.Connection.ReadStreamEventsBackwardAsync(
        _routesStream,
        start: _routesCheckpoint - 1,
        count: _key.Type.ResumeAlgorithm.GetNextBatchSize(_batchIndex),
        resolveLinkTos: true);

      if(result.Status != SliceReadStatus.Success)
      {
        throw new Exception($"Unexpected result when reading {_routesStream} to resume: {result.Status}");
      }

      return result;
    }
  }
}