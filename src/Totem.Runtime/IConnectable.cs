using System.Threading;
using System.Threading.Tasks;

namespace Totem.Runtime
{
  /// <summary>
  /// Describes a stateful connection to a resource
  /// </summary>
  public interface IConnectable
  {
    ConnectionState State { get; }

    Task Connect(CancellationToken cancellationToken = default(CancellationToken));

    Task Connect(IConnectable connection);

    Task Disconnect();
  }
}