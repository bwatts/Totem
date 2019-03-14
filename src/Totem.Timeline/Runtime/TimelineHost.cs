using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Totem.Runtime;
using Totem.Runtime.Hosting;

namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// Hosts the timeline as a service in the .NET runtime
  /// </summary>
  public class TimelineHost : ConnectedService, ITimelineObserver
  {
    readonly ITimelineDb _db;
    readonly ScheduleHost _schedule;
    readonly FlowHost _flows;

    public TimelineHost(IServiceProvider services, ITimelineDb db)
    {
      _db = db;

      _schedule = new ScheduleHost(db);
      _flows = new FlowHost(services, db);
    }

    protected override async Task Open()
    {
      // Tracking the database doesn't connect it immediately - connect before resuming the subscription

      await _db.Connect(this);

      Track(_db);
      Track(_flows);
      Track(await ResumeSubscription());
    }

    public async Task OnNext(TimelinePoint point)
    {
      LogNext(point);

      await _flows.OnNext(point);
      await _schedule.OnNext(point);
    }

    public void OnDropped(string reason, Exception error)
    {
      if(error != null)
      {
        Log.Error(error, "Dropped timeline subscription ({Reason})", reason);
      }
      else
      {
        Log.Debug("Dropped timeline subscription ({Reason})", reason);
      }
    }

    async Task<IConnectable> ResumeSubscription()
    {
      var resumeInfo = await _db.Subscribe(this);

      await _flows.Resume(resumeInfo.Routes);

      _schedule.Resume(resumeInfo.Schedule);

      return resumeInfo.Subscription;
    }

    void LogNext(TimelinePoint point)
    {
      var message = "[timeline] #";

      var args = new List<object>();

      if(point.Cause.IsNone)
      {
        message += "-";
      }
      else
      {
        message += "{Cause}";

        args.Add(point.Cause.ToInt64());
      }

      message += " ++ #{Position} {Type}";

      args.Add(point.Position.ToInt64());
      args.Add(point.Type);

      if(point.Scheduled)
      {
        message += " @ {When}";

        args.Add(point.WhenOccurs);
      }

      if(point.Topic != null)
      {
        message += " ({Topic}" + (point.IsImmediateGiven() ? " G" : "") + ")";

        args.Add(point.Topic);
      }

      Log.Debug(message, args.ToArray());
    }
  }
}