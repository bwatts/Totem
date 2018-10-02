using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace Totem.Timeline.EventStore.DbOperations
{
  /// <summary>
  /// Reads the events for which to set timers when resuming
  /// </summary>
  internal class ReadResumeScheduleCommand
  {
    readonly Many<TimelinePoint> _points = new Many<TimelinePoint>();
    readonly EventStoreContext _db;
    readonly ResumeAlgorithm _algorithm;
    readonly Many<long> _schedule;
    readonly long _scheduleFirst;
    readonly long _scheduleLast;
    readonly string _stream;
    long _streamCheckpoint;
    int _batchIndex;

    internal ReadResumeScheduleCommand(EventStoreContext db, Many<long> schedule)
    {
      _db = db;
      _schedule = schedule;

      _scheduleFirst = schedule.First();
      _scheduleLast = schedule.Last();

      _stream = _db.Area.GetScheduleStream();
      _streamCheckpoint = StreamPosition.End;

      // There is overhead in piping a resume algorithm from configuration. The default should work until
      // we experience otherwise.

      _algorithm = new ResumeAlgorithm();
    }

    internal async Task<Many<TimelinePoint>> Execute()
    {
      while(await ReadNextBatch())
      {
        _batchIndex++;
      }

      return _points;
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
        _streamCheckpoint = e.Link.EventNumber;

        var areaPosition = e.Event.EventNumber;

        if(areaPosition < _scheduleFirst)
        {
          return false;
        }

        if(areaPosition <= _scheduleLast && _schedule.Contains(areaPosition))
        {
          _points.Write.Insert(0, _db.ReadAreaPoint(e));
        }
      }

      return !batch.IsEndOfStream;
    }

    async Task<StreamEventsSlice> ReadBatch()
    {
      var result = await _db.Connection.ReadStreamEventsBackwardAsync(
        _stream,
        _streamCheckpoint,
        _algorithm.GetNextBatchSize(_batchIndex),
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
  }
}