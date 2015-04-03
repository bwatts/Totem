using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Nancy.Owin;
using Totem.Runtime;

namespace Totem.Web
{
	/// <summary>
	/// Registers the elements of Totem web APIs
	/// </summary>
	public class WebArea : RuntimeArea<WebAreaView>
	{
		protected override void RegisterArea()
		{
			Register(c => new WebHost(c.Resolve<IEnumerable<IWebApp>>().ToList()))
			.SingleInstance();

			Register(c => new WebApiApp(
				c.Resolve<ILifetimeScope>(),
				new NancyOptions(),
				AreaType,
				Settings.WebApiAppBindings))
			.As<IWebApp>()
			.SingleInstance();

			Register(c => new PushApp(c.Resolve<ILifetimeScope>(), Settings.PushAppBindings))
			.As<IWebApp>()
			.SingleInstance();

			RegisterType<ErrorHandler>().As<IErrorHandler>().SingleInstance();
		}

		public override IConnectable ResolveConnection(ILifetimeScope scope)
		{
			return scope.Resolve<WebHost>();
		}
	}
}