using StreamsDB.Driver;
using System.Threading;
using System.Threading.Tasks;
using Totem.Timeline.Client;

namespace Totem.Timeline.StreamsDb.Integration
{
  public class IntegrationSubscriber: IIntegrationSubscriber
  {
    private readonly IClientDb _clientDb;
    private readonly StreamsDbContext _context;

    public IntegrationSubscriber(IClientDb clientDb, StreamsDbContext context)
    {
      _clientDb = clientDb;
      _context = context;
    }

    public void Synchronize()
    {
      var from = GlobalPosition.Begin;

      Task.Run(async () =>
      {
        IGlobalSlice slice;

        do
        {
          slice = await _context.Client.DB().ReadGlobalForward(from, 100);

          foreach (var message in slice.Messages)
          {
            await Handle(message);
          }

          // set next position to read from
          from = slice.Next;
        }
        while (slice.HasNext);
      });
    }

    private async Task Handle(Message message)
    {
      if (message.Type.StartsWith("timeline:"))
      {
        var type = _context.ReadEventType(message);
        var e = _context.ReadEvent(message, type);
        await _clientDb.WriteEvent(e);
      }      
    }
  }
}
