using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// A call to a When method of a <see cref="Flow"/>
  /// </summary>
  public class WhenCall : FlowCall
	{
		internal WhenCall(
			Flow flow,
			TimelinePoint point,
			IDependencySource dependencies,
			ClaimsPrincipal principal,
			CancellationToken cancellationToken)
      : base(flow, point)
		{
			Dependencies = dependencies;
			Principal = principal;
			CancellationToken = cancellationToken;
		}

		public readonly IDependencySource Dependencies;
		public readonly ClaimsPrincipal Principal;
		public readonly CancellationToken CancellationToken;

    public override string ToString() => $"{Flow}.When({Point})";

    public Task Make()
    {
      return Flow.Type.CallWhen(Flow, Point, Dependencies);
    }
  }
}