using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Autofac;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Logging;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Conventions;
using Nancy.Owin;
using Owin;
using Totem.Http;
using Totem.IO;
using Totem.Runtime;
using Totem.Runtime.Map;
using Totem.Runtime.Timeline;

namespace Totem.Web
{
	/// <summary>
	/// An HTTP-bound API composed by OWIN and Nancy
	/// </summary>
	public class WebApp : AutofacNancyBootstrapper, IWebApp, ITaggable, IRootPathProvider
	{
		public WebApp(WebAppContext context)
		{
			Context = context;
			Tags = new Tags();
		}

		protected readonly WebAppContext Context;

		Tags ITaggable.Tags { get { return Tags; } }
		protected Tags Tags { get; private set; }
		protected IClock Clock { get { return Notion.Traits.Clock.Get(this); } }
		protected ILog Log { get { return Notion.Traits.Log.Get(this); } }
		protected RuntimeMap Runtime { get { return Notion.Traits.Runtime.Get(this); } }

		//
		// Configuration
		//

		protected override IRootPathProvider RootPathProvider
		{
			get { return this; }
		}

		public string GetRootPath()
		{
			return Context.UIFolder.ToString();
		}

		protected override void ConfigureConventions(NancyConventions nancyConventions)
		{
			base.ConfigureConventions(nancyConventions);

			new UIConventions(this, nancyConventions).Configure();
		}

		private sealed class UIConventions
		{
			private readonly WebApp _app;
			private readonly NancyConventions _conventions;

			internal UIConventions(WebApp app, NancyConventions conventions)
			{
				_app = app;
				_conventions = conventions;
			}

			internal void Configure()
			{
				FindViewsUnderUI();

				CoerceAcceptHeaders();

				ServeStaticContent();
			}

			private void FindViewsUnderUI()
			{
				_conventions.ViewLocationConventions.Clear();

				_conventions.ViewLocationConventions.Add((viewName, model, context) =>
				{
					return viewName;
				});
			}

			private void CoerceAcceptHeaders()
			{
				_conventions.AcceptHeaderCoercionConventions.Add((acceptHeaders, context) =>
				{
					return new[]
					{
						Tuple.Create("application/json", 1m),
						Tuple.Create("text/html", 0.9m),
						Tuple.Create("*/*", 0.8m)
					};
				});
			}

			private void ServeStaticContent()
			{
				ServeStaticContent("css");
				ServeStaticContent("images");
				ServeStaticContent("js");
				ServeStaticContent("references");
			}

			private void ServeStaticContent(string requestPath)
			{
				_conventions.StaticContentsConventions.Add(
					StaticContentConventionBuilder.AddDirectory(requestPath, GetContentFolder(requestPath)));
			}

			private string GetContentFolder(string requestPath)
			{
				return _app.Context.ContentFolder.Then(FolderResource.From(requestPath)).ToString();
			}
		}

		//
		// Lifecycle
		//

		public virtual IDisposable Start()
		{
			var startOptions = GetStartOptions();

			var instance = WebApp.Start(startOptions, Startup);

			if(startOptions.Urls.Count == 1)
			{
				 Log.Info("[web] Started API server at {Binding:l}", startOptions.Urls[0]);
			}
			else
			{
				Log.Info("[web] Started API server at {Bindings}", startOptions.Urls);
			}

			return instance;
		}

		protected virtual StartOptions GetStartOptions()
		{
			var options = new StartOptions();

			foreach(var binding in Context.Bindings)
			{
				options.Urls.Add(binding.ToString());
			}

			return options;
		}

		protected virtual void Startup(IAppBuilder builder)
		{
			builder.UseNancy(GetNancyOptions());

			builder.SetLoggerFactory(new LogAdapter());
		}

		protected virtual NancyOptions GetNancyOptions()
		{
			return new NancyOptions { Bootstrapper = this };
		}

		//
		// Composition
		//

		protected override ILifetimeScope GetApplicationContainer()
		{
			return Context.Scope;
		}

		protected override IEnumerable<INancyModule> GetAllModules(ILifetimeScope container)
		{
			return
				from apiType in Context.ApiTypes
				select (INancyModule) container.Resolve(apiType.DeclaredType);
		}

		protected override INancyModule GetModule(ILifetimeScope container, Type moduleType)
		{
			return (INancyModule) container.Resolve(moduleType);
		}

		protected override void ConfigureRequestContainer(ILifetimeScope container, NancyContext context)
		{
			base.ConfigureRequestContainer(container, context);

			var module = new BuilderModule();

			module.Register(c => new WebApiCall(
				HttpLink.From(context.Request.Url.ToString()),
				HttpAuthorization.From(context.Request.Headers.Authorization),
				WebApiCallBody.From(context.Request.Headers.ContentType, () => context.Request.Body)))
			.InstancePerRequest();

			module.Update(container.ComponentRegistry);
		}

		//
		// Requests
		//

		protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
		{
			pipelines.BeforeRequest += pipelineContext => BeforeRequest(container, pipelineContext);

			pipelines.OnError += (pipelineContext, exception) => OnRequestError(container, pipelineContext, exception);
		}

		private Response BeforeRequest(ILifetimeScope container, NancyContext context)
		{
			SetCallItem(container, context);

			return context.Response;
		}

		private Response OnRequestError(ILifetimeScope container, NancyContext context, Exception exception)
		{
			Log.Error(exception, "[web] Request error in {Url:l}", context.Request.Url);

			return container.Resolve<IErrorHandler>().CreateResponse(context, exception);
		}

		private static void SetCallItem(ILifetimeScope container, NancyContext context)
		{
			context.Items[WebApi.CallItemKey] = container.Resolve<WebApiCall>();

			IViewDb views;
			ITimeline timeline;

			if(container.TryResolve<IViewDb>(out views))
			{
				context.Items[WebApi.ViewsItemKey] = views;
			}

			if(container.TryResolve<ITimeline>(out timeline))
			{
				context.Items[WebApi.TimelineItemKey] = timeline;
			}
		}

		private sealed class LogAdapter : Notion, ILoggerFactory, ILogger
		{
			public ILogger Create(string name)
			{
				return this;
			}

			public bool WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
			{
				var level = GetLevel(eventType);

				// Reduce noise - OwinHttpListener throws exceptions when shutting down

				var canWrite = Log.IsAt(level)
					&& !(exception is ObjectDisposedException)
					&& !(exception is Win32Exception);

				if(canWrite)
				{
					Log.At(level, Text.Of(() => "[web] " + formatter(state, exception)));
				}

				return canWrite;
			}

			private static LogLevel GetLevel(TraceEventType type)
			{
				switch(type)
				{
					case TraceEventType.Verbose:
						return LogLevel.Verbose;
					case TraceEventType.Warning:
						return LogLevel.Warning;
					case TraceEventType.Error:
						return LogLevel.Error;
					case TraceEventType.Critical:
						return LogLevel.Fatal;
					default:
						return LogLevel.Debug;
				}
			}
		}
	}
}