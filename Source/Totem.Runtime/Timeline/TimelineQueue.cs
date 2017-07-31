﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// A queue of timeline points with asynchronous dequeues for the whole timeline
  /// </summary>
  internal sealed class TimelineQueue : Connection
	{
    readonly Subject<TimelineMessage> _messages = new Subject<TimelineMessage>();
    readonly Subject<TimelinePoint> _schedulePoints = new Subject<TimelinePoint>();
    readonly ITimelineDb _db;
    readonly TimelineSchedule _schedule;
    readonly TimelineFlowSet _flows;
		readonly TimelineRequestSet _requests;
    Task _resumeTask;
    TimelinePushSet _pushSet;

    internal TimelineQueue(
      ITimelineDb db,
      TimelineSchedule schedule,
      TimelineFlowSet flows,
      TimelineRequestSet requests)
		{
      _db = db;
			_schedule = schedule;
			_flows = flows;
			_requests = requests;

      _pushSet = new TimelinePushSet(this);
		}

    protected override void Open()
    {
      ObserveMessages();

      _resumeTask = Resume();
    }

    void ObserveMessages()
    {
      Track(_messages
        .ObserveOn(TimelineScheduler.Instance)
        .Subscribe(message =>
      {
        _flows.Push(message);
        _requests.Push(message);
      }));

      Track(_schedulePoints
        .ObserveOn(TimelineScheduler.Instance)
        .Subscribe(_schedule.Push));
    }

    async Task Resume()
    {
      await Task.Delay(500);

      try
      {
        var info = await _db.ReadResumeInfo();

        if(info.EventCount == 0)
        {
          Log.Info("[timeline] Resumed activity");
        }
        else
        {
          _flows.ResumeWith(info);

          ResumeWith(info);
        }
      }
      finally
      {
        _resumeTask = null;
      }
    }

    void ResumeWith(ResumeInfo info)
    {
      Log.Verbose(
        "[timeline] Resuming activity with {FlowCount} "
        + Text.Pluralized(info.Flows.Count, "flow")
        + " totaling {EventCount} "
        + Text.Pluralized(info.EventCount, "event"),
        info.Flows.Count,
        info.EventCount);

      var batchCount = 0;
      var pointCount = 0;
      var firstPosition = TimelinePosition.None;
      var lastPosition = TimelinePosition.None;

      foreach(var batch in info)
      {
        batchCount += 1;
        pointCount += batch.Points.Count;

        Log.Verbose(
          "[timeline] Resuming event batch #{BatchCount} spanning {First:l}-{Last:l} ({PointCount} {PercentComplete:#0.00%})",
          batchCount,
          batch.FirstPosition,
          batch.LastPosition,
          pointCount,
          pointCount / (decimal) info.EventCount);

        if(firstPosition == TimelinePosition.None)
        {
          firstPosition = batch.FirstPosition;
        }

        lastPosition = batch.LastPosition;

        foreach(var point in batch.Points)
        {
          _messages.OnNext(point.Message);

          if(point.OnSchedule)
          {
            Log.Verbose(
              "[timeline] Resuming timer for {Point:l} @ {When}",
              point.Message.Point,
              point.Message.Point.Event.When);

            _schedulePoints.OnNext(point.Message.Point);
          }
        }
      }

      Log.Info(
        "[timeline] Resumed activity with {BatchCount} "
        + Text.Pluralized(batchCount, "batch", "batches")
        + " ({PointCount} "
        + Text.Pluralized(pointCount, "point")
        + ") spanning {First:l}-{Last:l}",
        batchCount,
        pointCount,
        firstPosition,
        lastPosition);
    }

    //
    // Push
    //

    internal TimelinePush StartPush() =>
      _pushSet.StartPush();

    internal void PushMessage(TimelineMessage message)
    {
      _messages.OnNext(message);

      if(message.Point.Scheduled)
      {
        _schedulePoints.OnNext(message.Point);
      }
    }
  }
}