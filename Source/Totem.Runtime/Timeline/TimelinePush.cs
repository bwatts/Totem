using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// A transaction pushing one or more events to the timeline
  /// </summary>
  internal sealed class TimelinePush : Notion, IDisposable
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
      Messages.Write.Add(message);

      End();
    }

    internal void Commit(Many<TimelineMessage> messages)
    {
      Messages.Write.AddRange(messages);

      End();
    }

    public void Dispose()
    {
      if(!Done)
      {
        End();
      }
    }

    void End()
    {
      Done = true;

      _set.EndPush();
    }
  }
}