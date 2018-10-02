using System.Threading.Tasks;

namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// Describes the algorithm and secrets that authenticate timeline users
  /// </summary>
  public interface IUserDb
  {
    Task<User> Authenticate(Id userId);
  }
}