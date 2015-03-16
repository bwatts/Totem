using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Owin;
using Totem.Http;
using Totem.Runtime;

namespace Totem.Web
{
	/// <summary>
	/// An HTTP-bound push application composed by OWIN and SignalR
	/// </summary>
	public sealed class PushApplication : Notion, IWebApplication
	{
		private readonly ILifetimeScope _scope;

		public PushApplication(HttpResource resource, ILifetimeScope scope)
		{
			Resource = resource;
			_scope = scope;
		}

		public HttpResource Resource { get; private set; }

		public void Start(IAppBuilder builder)
		{
			GlobalHost.HubPipeline.AddModule(new LoggingPipelineModule());

			builder.MapHubs(new HubConfiguration
			{
				EnableDetailedErrors = true,
				Resolver = new PushDependencyResolver(_scope)
			});
		}

		private sealed class LoggingPipelineModule : HubPipelineModule, ITaggable
		{
			public LoggingPipelineModule()
			{
				Tags = new Tags();
			}

			public Tags Tags { get; private set; }
			private ILog Log { get { return Notion.Traits.Log.Get(this); } }

			protected override void OnIncomingError(Exception ex, IHubIncomingInvokerContext context)
			{
				Log.Error(ex, "An exception occurred in the SignalR pipeline");

				base.OnIncomingError(ex, context);
			}
		}

		// Adapted from http://code.google.com/p/autofac/source/browse/Core/Source/Autofac.Integration.SignalR/AutofacDependencyResolver.cs

		private sealed class PushDependencyResolver : DefaultDependencyResolver
		{
			private readonly ILifetimeScope _scope;

			internal PushDependencyResolver(ILifetimeScope scope)
			{
				_scope = scope;
			}

			public override object GetService(Type serviceType)
			{
				return _scope.ResolveOptional(serviceType) ?? base.GetService(serviceType);
			}

			public override IEnumerable<object> GetServices(Type serviceType)
			{
				var enumerableServiceType = typeof(IEnumerable<>).MakeGenericType(serviceType);

				var instance = (IEnumerable<object>) _scope.Resolve(enumerableServiceType);

				return instance.Any() ? instance : base.GetServices(serviceType);
			}
		}
	}
}