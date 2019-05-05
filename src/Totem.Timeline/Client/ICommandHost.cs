using System.Collections.Generic;
using System.Threading.Tasks;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// Describes the hosting of commands executing in the timeline client
  /// </summary>
  public interface ICommandHost
  {
    Task<TResponse> Execute<TResponse>(Command command, IEnumerable<ICommandWhen<TResponse>> whens);
  }
}