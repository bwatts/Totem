using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// Specifies the root path of the decorated web API
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class WebApiRootAttribute : Attribute
	{
		public WebApiRootAttribute(string path)
		{
			Path = LinkPath.From(path);
		}

		public readonly LinkPath Path;

		public override string ToString()
		{
			return Path.ToText();
		}
	}
}