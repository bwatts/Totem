using System;
using System.Linq;
using System.Threading.Tasks;
using StreamsDB.Driver;

namespace Totem.Timeline.StreamsDb.DbOperations
{
  /// <summary>
  /// Reads the events for which to set timers when resuming
  /// </summary>
  internal class ReadResumeScheduleCommand
  {
    readonly Many<TimelinePoint> _points = new Many<TimelinePoint>();
    readonly StreamsDbContext _context;
    readonly ResumeAlgorithm _algorithm;
    readonly Many<long> _schedule;
    readonly long _scheduleFirst;
    readonly long _scheduleLast;
    long _readCheckpoint;
    int _batchIndex;

    internal ReadResumeScheduleCommand(StreamsDbContext context, Many<long> schedule)
    {
      _context = context;
      _schedule = schedule;

      _scheduleFirst = schedule.First();
      _scheduleLast = schedule.Last();

      _readCheckpoint = -1;

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
      if(_readCheckpoint == 0)
      {
        return false;
      }

      var batch = await ReadBatch();

      foreach(var e in batch.Messages)
      {
        _readCheckpoint = e.Position;

        var areaPosition = e.Position;

        if(areaPosition < _scheduleFirst)
        {
          return false;
        }

        if(areaPosition <= _scheduleLast && _schedule.Contains(areaPosition))
        {
          _points.Write.Insert(0, _context.ReadAreaPoint(e));
        }
      }

      return !batch.HasNext;
    }

    async Task<IStreamSlice> ReadBatch()
    {
      var result = await _context.Client.DB().ReadStreamBackward(
        TimelineStreams.GetScheduleStream(_context.AreaName),
        _readCheckpoint,
        _algorithm.GetNextBatchSize(_batchIndex));

      
      return result;
    }
  }
}