using System;
using System.Threading.Tasks;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// Describes an observer that appear on a timeline client subscription
  /// </summary>
  public interface IClientObserver
  {
    Task OnNext(TimelinePoint point);

    Task OnDropped(string reason, Exception error);

    Task OnCommandFailed(Id commandId, string error);

    Task OnQueryChanged(QueryETag query);

    Task OnQueryStopped(QueryETag query, string error);
  }
}