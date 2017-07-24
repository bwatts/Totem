using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// A transaction pushing one or more events to the timeline
  /// </summary>
  internal sealed class TimelinePush : IDisposable
  {
    readonly TimelinePushSet _set;
    readonly List<TimelinePush> _group;

    internal TimelinePush(TimelinePushSet set, List<TimelinePush> group)
    {
      _set = set;
      _group = group;

      foreach(var other in group)
      {
        other._group.Add(this);
      }
    }

    internal bool Done { get; private set; }
    internal readonly Many<TimelineMessage> Messages = new Many<TimelineMessage>();

    internal bool GroupDone() =>
      Done && _group.All(other => other.Done);

    internal void Commit(TimelineMessage message)
    {
      Done = true;

      Messages.Write.Add(message);

      _set.EndPush(this);
    }

    internal void Commit(Many<TimelineMessage> messages)
    {
      Done = true;

      Messages.Write.AddRange(messages);

      _set.EndPush(this);
    }

    public void Dispose()
    {
      if(!Done)
      {
        Done = true;

        _set.EndPush(this);
      }
    }
  }
}