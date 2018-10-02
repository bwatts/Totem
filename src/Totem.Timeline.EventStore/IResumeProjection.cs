using System.Threading.Tasks;

namespace Totem.Timeline.EventStore
{
  /// <summary>
  /// Describes the projection installed to track the set of flows to resume
  /// </summary>
  public interface IResumeProjection
  {
    Task Synchronize();
  }
}