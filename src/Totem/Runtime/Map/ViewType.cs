using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A .NET type representing the state of a timeline query at a specific point
	/// </summary>
	public sealed class ViewType : RuntimeType
	{
		public ViewType(RuntimeTypeRef type) : base(type)
		{}

		public QueryType Query { get; private set; }

		internal void RegisterQuery(QueryType query)
		{
			Expect(Query).IsNull(Text.Of("Query {0} is already associated with view {1}", Query, this));

			Query = query;
		}
	}
}