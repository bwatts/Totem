using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Totem.Runtime
{
  /// <summary>
  /// A user or process establishing a security context with a runtime service
  /// </summary>
  public class Client
  {
    public Client()
    {
      Id = Id.Unassigned;
      Principal = new ClaimsPrincipal();
    }

    public Client(Id id, ClaimsPrincipal principal)
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