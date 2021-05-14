using System;
using System.Threading.Tasks;

namespace Totem.Routes
{
    public interface IRouteStoreTransaction : IDisposable
    {
        IRoute Route { get; }
        IRouteContext<IEvent> Context { get; }

        Task CommitAsync();
    }
}