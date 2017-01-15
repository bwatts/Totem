using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
	/// A call to a .Start method defined by a <see cref="Timeline.Request"/>
	/// </summary>
  public class RequestStartCall
  {
    public RequestStartCall(Client client, IDependencySource dependencies)
    {
      Client = client;
      Dependencies = dependencies;
    }

    public readonly Client Client;
    public readonly IDependencySource Dependencies;

    internal async Task<Event> Make(Request request)
    {
      try
      {
        request.Client = Client;

        Authorize(request);

        return await CallStart(request);
      }
      finally
      {
        request.Client = null;
      }
    }

    void Authorize(Request request)
    {
      if(!request.Authorize())
      {
        throw new RequestDeniedException();
      }
    }

    Task<Event> CallStart(Request request)
    {
      var type = (RequestType) request.Context.Type;

      return type.StartMethod.Call(request, Dependencies);
    }
  }
}