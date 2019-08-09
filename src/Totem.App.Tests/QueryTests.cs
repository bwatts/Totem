using System.Threading.Tasks;
using Totem.App.Tests.Hosting;
using Totem.Timeline;

namespace Totem.App.Tests
{
  /// <summary>
  /// A set of tests applied to a query of the specified type
  /// </summary>
  /// <typeparam name="TQuery">The type of query under test</typeparam>
  public abstract class QueryTests<TQuery> : AppTests<QueryAppHost> where TQuery : Query
  {
    internal override QueryAppHost CreateHost() =>
      new QueryAppHost(typeof(TQuery));

    protected async Task Append(Event e)
    {
      var host = await GetOrStartHost();

      await host.Append(e);
    }

    protected Task<TQuery> AppendAndGet(Event e, ExpectTimeout timeout = null) =>
      AppendAndGet(Id.Unassigned, e, timeout);

    protected async Task<TQuery> AppendAndGet(Id queryId, Event e, ExpectTimeout changeTimeout = null)
    {
      var host = await GetOrStartHost();

      return await host.AppendAndGet<TQuery>(queryId, e, changeTimeout ?? ExpectTimeout.Default);
    }
  }
}