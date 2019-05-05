using System;
using System.Threading.Tasks;
using Totem.Runtime;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// Describes the database through which a timeline client executes commands
  /// </summary>
  public interface ICommandDb : IConnectable
  {
    Task<IDisposable> Subscribe(ITimelineObserver observer);

    Task<TimelinePosition> WriteEvent(Event e);
  }
}