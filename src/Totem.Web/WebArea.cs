using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Nancy;
using Nancy.Serialization.JsonNet;
using Newtonsoft.Json;
using Totem.IO;
using Totem.Runtime;
using Totem.Runtime.Json;

namespace Totem.Web
{
	/// <summary>
	/// Registers the elements of the Totem web host
	/// </summary>
	public class WebArea : RuntimeArea
	{
		protected override void RegisterArea()
		{
			RegisterType<WebHost>().SingleInstance();

			RegisterType<ErrorHandler>().As<IErrorHandler>().SingleInstance();

			Register(c => ErrorDetail.StackTrace).SingleInstance();

			Register(c => new JsonNetSerializer(new TotemSerializerSettings().CreateSerializer()))
			.As<ISerializer>()
			.SingleInstance();
		}

		public override IConnectable Compose(ILifetimeScope scope)
		{
			return scope.Resolve<WebHost>();
		}

		private sealed class WebHost : Connection
		{
			private readonly IEnumerable<IWebApp> _apps;

			public WebHost(IEnumerable<IWebApp> apps)
			{
				_apps = apps;
			}

			protected override void Open()
			{
				Track(_apps.Select(app => app.Start()));
			}
		}
	}
}