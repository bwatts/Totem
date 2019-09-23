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

    protected Task<TQuery> GetQuery(ExpectTimeout timeout = null) =>
      GetQuery(Id.Unassigned, timeout);

    protected async Task<TQuery> GetQuery(Id instanceId, ExpectTimeout timeout = null)
    {
      var host = await GetOrStartHost();

      return await host.GetQuery<TQuery>(instanceId, timeout ?? ExpectTimeout.Default);
    }

    protected Task ExpectDone(ExpectTimeout timeout = null) =>
      ExpectDone(Id.Unassigned, timeout);

    protected async Task ExpectDone(Id instanceId, ExpectTimeout timeout = null)
    {
      var host = await GetOrStartHost();

      await host.ExpectDone(instanceId, timeout ?? ExpectTimeout.Default);
    }
  }
}