using System.Threading.Tasks;

namespace Totem.EventBus
{
  public interface IEventBusContext
  {
    Task Connect();
    void Disconnect();
  }
}