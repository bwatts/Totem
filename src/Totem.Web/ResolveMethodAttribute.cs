using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Web
{
	/// <summary>
	/// Indicates the decorated <see cref="WebApi"/> resolves API instances via the specified static method.
	/// 
	/// Resolve methods are public, static, accept a single <see cref="ILifetimeScope" /> parameter, and return <see cref="WebApi"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ResolveMethodAttribute : Attribute
	{
		public ResolveMethodAttribute(string methodName)
		{
			MethodName = methodName;
		}

		public readonly string MethodName;
	}
}