using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime;

namespace Totem.Http
{
	/// <summary>
	/// Specifies the HTTP resource and/or resolve method of the decorated web API
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class WebApiAttribute : Attribute
	{
		public WebApiAttribute(string resourcePath = "/", string resolveMethod = "")
		{
			Resource = HttpResource.From(resourcePath);
			ResolveMethod = resolveMethod;
		}

		public readonly HttpResource Resource;
		public readonly string ResolveMethod;

		public override string ToString()
		{
			return Resource.ToText();
		}
	}
}