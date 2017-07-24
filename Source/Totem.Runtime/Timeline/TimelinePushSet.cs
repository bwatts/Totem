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

    internal void EndPush(TimelinePush push)
    {
      lock(_pushes)
      {
        var messages = new List<TimelineMessage>();

        foreach(var donePush in _pushes.Where(p => p.GroupDone()).ToList())
        {
          _pushes.Remove(donePush);

          foreach(var message in donePush.Messages)
          {
            messages.Add(message);
          }
        }

        foreach(var message in messages.OrderBy(m => m.Point.Position))
        {
          _queue.PushMessage(message);
        }
      }
    }
  }
}