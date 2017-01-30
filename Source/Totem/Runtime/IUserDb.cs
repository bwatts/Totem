using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime
{
  /// <summary>
  /// Describes the algorithm and secrets that authenticate runtime users
  /// </summary>
  public interface IUserDb
  {
    Task<User> Authenticate(Id userId);
  }
}