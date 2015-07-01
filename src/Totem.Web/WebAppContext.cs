using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Totem.Http;

namespace Totem.Web
{
	/// <summary>
	/// The context established for an HTTP-bound application
	/// </summary>
	public class WebAppContext
	{
		public WebAppContext(Many<HttpLink> bindings, ILifetimeScope scope)
		{
			Bindings = bindings;
			Scope = scope;
		}

		public readonly Many<HttpLink> Bindings;
		public readonly ILifetimeScope Scope;
	}
}