using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A call to a When method of a <see cref="Query"/>
	/// </summary>
	public sealed class QueryCall : FlowCall
	{
		public QueryCall(
			QueryType type,
			Query instance,
			TimelinePoint point,
			IDependencySource dependencies,
			ClaimsPrincipal principal,
			CancellationToken cancellationToken,
			IViewDb views)
			: base(type, instance, point, dependencies, principal, cancellationToken)
		{
			Views = views;
		}

		public readonly IViewDb Views;
	}
}