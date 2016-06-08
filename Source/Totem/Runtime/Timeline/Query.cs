using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A timeline presence that maintains some form of read model
	/// </summary>
	[SnapshotAlways]
	public abstract class Query : Flow
	{
		[Transient] protected new QueryType Type => (QueryType) base.Type;
	}
}