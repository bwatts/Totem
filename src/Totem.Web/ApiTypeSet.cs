using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map;

namespace Totem.Web
{
	/// <summary>
	/// A set of types representing web APIs
	/// </summary>
	public class ApiTypeSet : RuntimeTypeSetCore<ApiType>
	{
		public ApiTypeSet(IEnumerable<ApiType> apis)
		{
			foreach(var api in apis)
			{
				Register(api);
			}
		}
	}
}