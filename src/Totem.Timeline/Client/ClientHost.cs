using System;
using System.Threading.Tasks;
using Totem.Runtime;
using Totem.Runtime.Hosting;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// Hosts a timeline client as a service in the .NET runtime
  /// </summary>
  internal sealed class ClientHost : ConnectedService, IClientObserver
  {
    readonly IClientDb _db;
    readonly CommandHost _commands;
    readonly QueryHost _queries;

    public ClientHost(IClientDb db, CommandHost commands, QueryHost queries)
    {
      _db = db;
      _commands = commands;
      _queries = queries;
    }

    protected override async Task Open()
    {
      await _db.Connect(this);

      Track(_db);
      Track(await _db.Subscribe(this));
    }

    public Task OnNext(TimelinePoint point) =>
      _commands.OnNext(point);

    public Task OnDropped(string reason, Exception error)
    {
      if(error != null)
      {
        Log.Error(error, "Dropped timeline subscription ({Reason})", reason);
      }
      else
      {
        Log.Debug("Dropped timeline subscription ({Reason})", reason);
      }

      // By this point, one or both subscriptions are dropped. Ensure both are.

      return Disconnect();
    }

    public Task OnCommandFailed(Id commandId, string error) =>
      _commands.OnFailed(commandId, error);

    public Task OnQueryChanged(QueryETag query) =>
      _queries.OnChanged(query);

    public Task OnQueryStopped(QueryETag query, string error) =>
      _queries.OnStopped(query, error);
  }
}