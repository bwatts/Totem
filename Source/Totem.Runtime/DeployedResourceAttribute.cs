using System;
using System.Collections.Generic;
using System.Linq;
using Totem.IO;

namespace Totem.Runtime
{
	/// <summary>
	/// Specifies an I/O resource copied during deployment
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public sealed class DeployedResourceAttribute : Attribute
	{
		public DeployedResourceAttribute(string resource)
		{
			Resource = IOResource.From(resource);
		}

		public readonly IOResource Resource;
	}
}