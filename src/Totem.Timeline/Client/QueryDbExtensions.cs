using System;
using System.Threading.Tasks;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// Extends <see cref="IQueryDb"/> with methods to read queries by type and identifier
  /// </summary>
  public static class QueryDbExtensions
  {
    public static Task<Query> ReadQuery(this IQueryDb db, Type type, Id id) =>
      db.ReadQuery(area => area.Queries[type].CreateKey(id));

    public static Task<Query> ReadQuery(this IQueryDb db, Type type) =>
      db.ReadQuery(type, Id.Unassigned);

    public static async Task<T> ReadQuery<T>(this IQueryDb db, Id id) where T : Query =>
      (T) await db.ReadQuery(typeof(T), id);

    public static Task<T> ReadQuery<T>(this IQueryDb db) where T : Query =>
      db.ReadQuery<T>(Id.Unassigned);

    public static Task<QueryContent> ReadQueryContent(this IQueryDb db, string etag, Type type, Id id) =>
      db.ReadQueryContent(area =>
      {
        if(!string.IsNullOrWhiteSpace(etag))
        {
          return QueryETag.From(etag, area);
        }

        var key = FlowKey.From(area.Queries[type], id);

        return QueryETag.From(key, TimelinePosition.None);
      });
  }
}