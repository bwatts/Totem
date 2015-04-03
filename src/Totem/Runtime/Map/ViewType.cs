using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A .NET type representing the state of a timeline query at a specific point
	/// </summary>
	public sealed class ViewType : RuntimeType
	{
		public ViewType(RuntimeTypeRef type) : base(type)
		{}
	}
}