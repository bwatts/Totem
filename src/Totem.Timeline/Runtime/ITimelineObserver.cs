using System;
using System.Threading.Tasks;

namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// Describes an observer of points that appear on a timeline subscription
  /// </summary>
  public interface ITimelineObserver
  {
    Task OnNext(TimelinePoint point);

    void OnDropped(string reason, Exception error);
  }
}