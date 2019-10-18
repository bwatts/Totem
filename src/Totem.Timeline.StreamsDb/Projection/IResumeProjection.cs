using System.Threading.Tasks;

namespace Totem.Timeline.StreamsDb
{
  /// <summary>
  /// Describes the projection installed to track the set of flows to resume
  /// </summary>
  public interface IResumeProjection
  {
    Task SynchronizeAsync();
  }
}