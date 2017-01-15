using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime
{
  /// <summary>
  /// Describes the algorithm and secrets that authenticate runtime clients
  /// </summary>
  public interface IClientAuthority
  {
    Task<Client> Authenticate(Id clientId);
  }
}