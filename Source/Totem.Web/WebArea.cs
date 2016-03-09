using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Nancy;
using Nancy.Serialization.JsonNet;
using Totem.Runtime;
using Totem.Runtime.Json;
using Totem.Web.Push;

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

			// https://code.google.com/p/autofac/wiki/SignalRIntegration
			//
			// "Hub registrations should be marked with ExternallyOwned to inform the container that the hubs
			// will be disposed by SignalR and not the container."

			RegisterType<PushHub>().InstancePerDependency().ExternallyOwned();

			RegisterType<PushContext>().As<IPushContext>().SingleInstance();

			// Do not explicitly register an implementation of IBodyDeserializer.
			//
			// The Nancy code around it uses Activator.CreateInstance, violating the promises of [Durable].
			//
			// WebApi types use ReadBody<T> overloads to deserialize content in the request body. This narrows
			// flexibility by assuming JSON content, but avoids exceptions relating to creating abstract classes.
			//
			// The Nancy.ModelBinding namespace remains unaffected. The tweaks involving IBodyDeserializer
			// essentially skipped the binding pipeline; I chose not to bend the abstraction to my will.
			//
			// If an API type chooses to use it, the Totem.Runtime.Json namespace will not be in effect.

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