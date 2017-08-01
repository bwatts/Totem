using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The set of transactions pushing events to the timeline
  /// </summary>
  internal sealed class TimelinePushSet : Notion
  {
    readonly List<TimelinePush> _pushes = new List<TimelinePush>();
    readonly TimelineQueue _queue;

    internal TimelinePushSet(TimelineQueue queue)
    {
      _queue = queue;
    }

    internal TimelinePush StartPush()
    {
      lock(_pushes)
      {
        var group = _pushes.Where(p => !p.Done).ToList();

        var push = new TimelinePush(this, group);

        _pushes.Add(push);

        TimelinePushMetrics.Open.Set(_pushes.Count);
        TimelinePushMetrics.Done.Set(_pushes.Count - group.Count);
        TimelinePushMetrics.Group.Set(group.Count);

        Log.Warning("[timeline-push] START       {Group:l}", Many.Of(group, push).Reverse().Select(p => p.Id));

        return push;
      }
    }

    internal void EndPush(TimelinePush push)
    {
      lock(_pushes)
      {
        var donePushes = TakeWhileGroupDone().ToList();

        Log.Warning("[timeline-push] END         {Group:l}", donePushes);

        var messages = new List<Tuple<string, TimelineMessage>>();

        foreach(var donePush in donePushes)
        {
          _pushes.Remove(donePush);

          foreach(var message in donePush.Messages)
          {
            messages.Add(Tuple.Create(donePush.Id, message));
          }
        }

        foreach(var message in messages.OrderBy(m => m.Item2.Point.Position))
        {
          Log.Warning("[timeline-push] PUSH        {Id:l} => {Position:l}", message.Item1, message.Item2.Point.Position);

          _queue.PushMessage(message.Item2);
        }

        TimelinePushMetrics.Open.Set(_pushes.Count);
      }
    }

    IEnumerable<TimelinePush> TakeWhileGroupDone()
    {
      while(_pushes.Count > 0 && _pushes[0].GroupDone())
      {
        yield return _pushes[0];

        _pushes.RemoveAt(0);
      }
    }
  }
}