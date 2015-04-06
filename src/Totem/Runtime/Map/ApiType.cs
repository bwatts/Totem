using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A .NET type representing a Totem web API
	/// </summary>
	public sealed class ApiType : RuntimeType
	{
		internal ApiType(RuntimeTypeRef type, LinkPath rootPath) : base(type)
		{
			RootPath = rootPath;
		}

		public readonly LinkPath RootPath;
	}
}