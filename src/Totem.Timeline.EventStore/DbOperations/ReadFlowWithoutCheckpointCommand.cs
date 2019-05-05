using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.EventStore.DbOperations
{
  /// <summary>
  /// Reads the <see cref="FlowResumeInfo"/> for a particular flow without a checkpoint
  /// </summary>
  internal class ReadFlowWithoutCheckpointCommand
  {
    readonly Many<TimelinePoint> _points = new Many<TimelinePoint>();
    readonly EventStoreContext _context;
    readonly FlowKey _key;
    readonly string _stream;
    Flow _flow;
    long? _streamCheckpoint;
    int _batchIndex;

    internal ReadFlowWithoutCheckpointCommand(EventStoreContext context, FlowKey key)
    {
      _context = context;
      _key = key;

      _stream = key.GetRoutesStream();
    }

    internal async Task<FlowResumeInfo> Execute()
    {
      while(await ReadNextBatch())
      {
        _batchIndex++;
      }

      return _flow == null
        ? new FlowResumeInfo.NotFound()
        : new FlowResumeInfo.Loaded(_flow, _points) as FlowResumeInfo;
    }

    async Task<bool> ReadNextBatch()
    {
      var batch = await ReadBatch();

      if(batch.Status == SliceReadStatus.StreamNotFound)
      {
        return false;
      }

      foreach(var e in batch.Events)
      {
        TryAddPoint(e);
      }

      return !batch.IsEndOfStream;
    }

    async Task<StreamEventsSlice> ReadBatch()
    {
      var result = await _context.Connection.ReadStreamEventsForwardAsync(
        _stream,
        NextBatchStart,
        NextBatchSize,
        resolveLinkTos: true);

      switch(result.Status)
      {
        case SliceReadStatus.StreamNotFound:
        case SliceReadStatus.Success:
          return result;
        default:
          throw new Exception($"Unexpected result when reading {_stream} to resume: {result.Status}");
      }
    }

    long NextBatchStart =>
      _streamCheckpoint.GetValueOrDefault(StreamPosition.Start);

    int NextBatchSize =>
      _key.Type.ResumeAlgorithm.GetNextBatchSize(_batchIndex);

    void TryAddPoint(ResolvedEvent e)
    {
      if(_flow != null)
      {
        AddPoint(e);
      }
      else
      {
        if(CanBeFirst(e))
        {
          CreateFlow();

          AddPoint(e);
        }
      }

      _streamCheckpoint = e.Link.EventNumber;
    }

    void AddPoint(ResolvedEvent e) =>
      _points.Write.Add(_context.ReadAreaPoint(e));

    bool CanBeFirst(ResolvedEvent e)
    {
      var type = _context.ReadEventType(e);

      var observation = _key.Type.Observations.Get(type);

      return observation.CanBeFirst;
    }

    void CreateFlow()
    {
      _flow = _key.Type.New();

      FlowContext.Bind(_flow, _key);
    }
  }
}