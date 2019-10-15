using System.Threading.Tasks;
using StreamsDB.Driver;
using Totem.Runtime;
using Totem.Runtime.Json;
using Totem.Threading;
using Totem.Timeline.Area;

namespace Totem.Timeline.EventStore
{
  /// <summary>
  /// The connection, JSON format, and area map in effect for the EventStore timeline
  /// </summary>
  public class EventStoreContext : Connection
  {
    TaskSource _connectInitially;

    public EventStoreContext(IJsonFormat json, AreaMap area)
    {
      Json = json;
      Area = area;
    }

    public StreamsDBClient Client;
    public readonly IJsonFormat Json;
    public readonly AreaMap Area;

    protected override Task Open()
    {
      return ConnectInitially();
    }

    protected override Task Close()
    {
      Client.Close();

      return base.Close();
    }

    async Task ConnectInitially()
    {
      _connectInitially = new TaskSource();

      try
      {
        Client = await StreamsDBClient.Connect("");

        await _connectInitially.Task;
      }
      finally
      {
        _connectInitially = null;
      }
    }
  }
}