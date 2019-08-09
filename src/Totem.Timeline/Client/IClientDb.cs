using System;
using System.Threading.Tasks;
using Totem.Runtime;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// Describes a database containing timeline data relevant to clients
  /// </summary>
  public interface IClientDb : IConnectable
  {
    Task<IDisposable> Subscribe(IClientObserver observer);

    Task<TimelinePosition> WriteEvent(Event e);

    Task<QueryState> ReadQuery(QueryETag etag);

    Task<Query> ReadQueryContent(FlowKey key);
  }
}