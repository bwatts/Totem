using System;
using System.Threading.Tasks;
using Totem.Timeline.Area;
using Totem.Timeline.Client;

namespace Totem.Timeline.EventStore.Client
{
  /// <summary>
  /// An EventStore database containing query data
  /// </summary>
  public sealed class QueryDb : IQueryDb
  {
    readonly AreaMap _area;
    readonly IClientDb _clientDb;

    public QueryDb(AreaMap area, IClientDb clientDb)
    {
      _area = area;
      _clientDb = clientDb;
    }

    public Task<Query> ReadQuery(Func<AreaMap, FlowKey> getKey) =>
      _clientDb.ReadQuery(getKey(_area));

    public Task<QueryContent> ReadQueryContent(Func<AreaMap, QueryETag> getETag) =>
      _clientDb.ReadQueryContent(getETag(_area));
  }
}