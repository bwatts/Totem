using System.Threading.Tasks;
using StreamsDB.Driver;

namespace Totem.EventBus.StreamsDb
{
  public class StreamsDbEventBusContext : IEventBusContext
  {
    private readonly string _connectionString;
    
    public string Stream { get; }
    public string Area { get; }
    public StreamsDBClient Client { get; private set; }

    public StreamsDbEventBusContext(string connectionString, string stream, string area)
    {
      _connectionString = connectionString;
      Stream = stream;
      Area = area;
    }

    public async Task Connect()
    {
      Client = await StreamsDBClient.Connect(_connectionString);
    }

    public void Disconnect()
    {
      throw new System.NotImplementedException();
    }
  }
}
