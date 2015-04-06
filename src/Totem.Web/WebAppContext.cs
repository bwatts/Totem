using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Totem.Http;
using Totem.IO;

namespace Totem.Web
{
	/// <summary>
	/// The context established for an HTTP-bound API
	/// </summary>
	public class WebAppContext
	{
		public WebAppContext(IReadOnlyList<HttpLink> bindings, ILifetimeScope scope)
		{
			Bindings = bindings;
			Scope = scope;
		}

		public readonly IReadOnlyList<HttpLink> Bindings;
		public readonly ILifetimeScope Scope;
	}
}