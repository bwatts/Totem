using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The set of transactions pushing events to the timeline
  /// </summary>
  internal sealed class TimelinePushSet
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

        return push;
      }
    }

    internal void EndPush()
    {
      lock(_pushes)
      {
        var messages =
          from groupDonePush in TakeWhileGroupDone()
          from message in groupDonePush.Messages
          orderby message.Point.Position
          select message;

        foreach(var message in messages)
        {
          _queue.PushMessage(message);
        }
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