using System.Threading.Tasks;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// Describes a response of the specified type initiated by an event
  /// </summary>
  /// <typeparam name="TResponse">The type of initiated response</typeparam>
  public interface ICommandWhen<TResponse>
  {
    bool CanRespond(TimelinePoint point);

    Task<TResponse> Respond(TimelinePoint point);
  }
}