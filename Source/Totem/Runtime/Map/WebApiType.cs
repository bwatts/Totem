using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A .NET type representing an instance of a web API bound to an HTTP request
	/// </summary>
	public sealed class WebApiType : RuntimeType
	{
		public WebApiType(RuntimeTypeRef type) : base(type)
		{}
	}
}