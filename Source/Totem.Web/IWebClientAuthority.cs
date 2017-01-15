using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Totem.Http;
using Totem.Runtime;

namespace Totem.Web
{
  /// <summary>
  /// Describes the algorithm and secrets that authenticate web clients
  /// </summary>
  public interface IWebClientAuthority
  {
    Task<Client> Authenticate(HttpAuthorization header);
	}
}