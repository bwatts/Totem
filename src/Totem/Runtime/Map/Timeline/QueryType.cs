using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .NET type representing a query on the timeline
	/// </summary>
	public sealed class QueryType : FlowType
	{
		internal QueryType(RuntimeTypeRef type, FlowConstructor constructor, ViewType view) : base(type, constructor)
		{
			View = view;

			view.RegisterQuery(this);
		}

		public readonly ViewType View;
	}
}