using System.Security.Claims;

namespace Totem
{
  /// <summary>
  /// A network entity establishing a security context with a runtime service
  /// </summary>
  public class User
  {
    public User()
    {
      Id = Id.Unassigned;
      Principal = new ClaimsPrincipal();
    }

    public User(Id id, ClaimsPrincipal principal)
    {
      Id = id;
      Principal = principal;
    }

    public Id Id { get; }
    public ClaimsPrincipal Principal { get; }

    public bool IsAnonymous => !Principal.Identity?.IsAuthenticated ?? true;
    public bool IsAuthenticated => Principal.Identity?.IsAuthenticated ?? false;
    public string Name => Principal.Identity?.Name ?? "";

    public override string ToString() => Name;
  }
}