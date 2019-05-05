using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Totem.Runtime.Json;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.EventStore.DbOperations
{
  /// <summary>
  /// Reads the <see cref="FlowResumeInfo"/> for a particular flow with a checkpoint
  /// </summary>
  internal class ReadFlowWithCheckpointCommand
  {
    readonly Many<TimelinePoint> _points = new Many<TimelinePoint>();
    readonly EventStoreContext _context;
    readonly FlowKey _key;
    readonly RecordedEvent _checkpoint;
    readonly string _stream;
    Flow _flow;
    long _areaCheckpoint;
    long _streamCheckpoint;
    int _batchIndex;

    internal ReadFlowWithCheckpointCommand(EventStoreContext context, FlowKey key, RecordedEvent checkpoint)
    {
      _context = context;
      _key = key;
      _checkpoint = checkpoint;

      _stream = key.GetRoutesStream();
    }

    internal async Task<FlowResumeInfo> Execute()
    {
      var metadata = ReadMetadata();

      if(metadata.ErrorPosition.IsSome)
      {
        return new FlowResumeInfo.Stopped(metadata.ErrorPosition);
      }

      BindFlow(metadata.Position);

      await ReadPoints();

      return new FlowResumeInfo.Loaded(_flow, _points);
    }

    CheckpointMetadata ReadMetadata() =>
      _context.Json.FromJsonUtf8<CheckpointMetadata>(_checkpoint.Metadata);

    void BindFlow(TimelinePosition position)
    {
      _flow = ReadData();

      FlowContext.Bind(_flow, _key, position, TimelinePosition.None);

      _areaCheckpoint = position.ToInt64();
    }

    Flow ReadData() =>
      (Flow) _context.Json.FromJsonUtf8(_checkpoint.Data, _key.Type.DeclaredType);

    async Task ReadPoints()
    {
      var result = await ReadLastRoute();

      if(result.Status == EventReadStatus.Success)
      {
        await ReadPoints(result.Event.Value);
      }
    }

    Task<EventReadResult> ReadLastRoute() =>
      _context.Connection.ReadEventAsync(_stream, StreamPosition.End, resolveLinkTos: true);

    async Task ReadPoints(ResolvedEvent lastRoute)
    {
      if(_areaCheckpoint < lastRoute.Event.EventNumber)
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

      _streamCheckpoint = e.Link.EventNumber;
    }

    async Task<bool> ReadNextBatch()
    {
      if(_streamCheckpoint == 0)
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
        _stream,
        NextBatchStart,
        NextBatchSize,
        resolveLinkTos: true);

      if(result.Status != SliceReadStatus.Success)
      {
        throw new Exception($"Unexpected result when reading {_stream} to resume: {result.Status}");
      }

      return result;
    }

    long NextBatchStart =>
      _streamCheckpoint - 1;

    int NextBatchSize =>
      _key.Type.ResumeAlgorithm.GetNextBatchSize(_batchIndex);
  }
}