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
    public RequestStartCall(User user, IDependencySource dependencies)
    {
      User = user;
      Dependencies = dependencies;
    }

    public readonly User User;
    public readonly IDependencySource Dependencies;

    internal async Task<Event> Make(Request request)
    {
      try
      {
        request.User = User;

        Authorize(request);

        return await CallStart(request);
      }
      finally
      {
        request.User = null;
      }
    }

    void Authorize(Request request)
    {
      if(!request.Authorize())
      {
        throw new UnauthorizedAccessException();
      }
    }

    Task<Event> CallStart(Request request)
    {
      var type = (RequestType) request.Context.Type;

      return type.StartMethod.Call(request, Dependencies);
    }
  }
}