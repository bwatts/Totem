using System;
using System.Threading.Tasks;
using Totem.Timeline.Area;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// Describes a database containing query data
  /// </summary>
  public interface IQueryDb
  {
    Task<Query> ReadQuery(Func<AreaMap, FlowKey> getKey);

    Task<QueryContent> ReadQueryContent(Func<AreaMap, QueryETag> getETag);
  }
}