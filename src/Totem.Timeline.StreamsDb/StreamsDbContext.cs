using System.Threading.Tasks;
using StreamsDB.Driver;
using Totem.Runtime;
using Totem.Runtime.Json;
using Totem.Threading;
using Totem.Timeline.Area;

namespace Totem.Timeline.StreamsDb
{
  /// <summary>
  /// The connection, JSON format, and area map in effect for the EventStore timeline
  /// </summary>
  public class StreamsDbContext : Connection
  {
    public StreamsDbContext(string connectionString, string areaName, IJsonFormat json, AreaMap area)
    {
      ConnectionString = connectionString;
      AreaName = areaName;
      Json = json;
      Area = area;
    }

    public StreamsDBClient Client;

    public readonly string ConnectionString;
    public readonly string AreaName;
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
      Client = await StreamsDBClient.Connect(ConnectionString);
    }
  }
}