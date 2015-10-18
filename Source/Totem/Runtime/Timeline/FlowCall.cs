using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A call to a When method of a <see cref="Flow"/>
	/// </summary>
	public class FlowCall : Notion
	{
		internal FlowCall(
			FlowType type,
			Flow instance,
			TimelinePoint point,
			IDependencySource dependencies,
			ClaimsPrincipal principal,
			CancellationToken cancellationToken)
		{
			Type = type;
			Instance = instance;
			Point = point;
			Dependencies = dependencies;
			Principal = principal;
			CancellationToken = cancellationToken;
		}

		public readonly FlowType Type;
		public readonly Flow Instance;
		public readonly TimelinePoint Point;
		public readonly IDependencySource Dependencies;
		public readonly ClaimsPrincipal Principal;
		public readonly CancellationToken CancellationToken;

		public static FlowCall From(
			FlowType type,
			Flow instance,
			TimelinePoint point,
			IDependencySource dependencies,
			ClaimsPrincipal principal,
			CancellationToken cancellationToken)
		{
			if(type.IsTopic)
			{
				return new TopicCall((TopicType) type, (Topic) instance, point, dependencies, principal, cancellationToken);
			}
			else if(type.IsQuery)
			{
				return new QueryCall((QueryType) type, (Query) instance, point, dependencies, principal, cancellationToken, dependencies.Resolve<IViewDb>());
      }
			else
			{
				return new FlowCall(type, instance, point, dependencies, principal, cancellationToken);
			}
		}
  }
}