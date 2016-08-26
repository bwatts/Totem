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
		public WebAppContext(Many<HttpLink> bindings, ILifetimeScope scope, bool enableCors)
		{
			Bindings = bindings;
			Scope = scope;
			EnableCors = enableCors;
		}

		public WebAppContext(HttpLink binding, ILifetimeScope scope, bool enableCors) : this(Many.Of(binding), scope, enableCors)
		{}

		public readonly Many<HttpLink> Bindings;
		public readonly ILifetimeScope Scope;
		public readonly bool EnableCors;
	}
}