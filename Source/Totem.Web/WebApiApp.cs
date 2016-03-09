using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Logging;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Owin;
using Owin;
using Totem.Http;
using Totem.Runtime;
using Totem.Runtime.Map;
using Totem.Runtime.Timeline;

namespace Totem.Web
{
	/// <summary>
	/// An HTTP-bound API composed by OWIN and Nancy
	/// </summary>
	public abstract class WebApiApp : AutofacNancyBootstrapper, IWebApp, ITaggable
	{
		protected WebApiApp(WebAppContext context)
		{
			Context = context;
			Tags = new Tags();
		}

		Tags ITaggable.Tags => Tags;
		protected Tags Tags { get; private set; }
		protected IClock Clock => Notion.Traits.Clock.Get(this);
		protected ILog Log => Notion.Traits.Log.Get(this);
		protected RuntimeMap Runtime => Notion.Traits.Runtime.Get(this);

		protected readonly WebAppContext Context;

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

		protected override abstract IEnumerable<INancyModule> GetAllModules(ILifetimeScope container);

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

			pipelines.AfterRequest += pipelineContext => AfterRequest(container, pipelineContext);
		}

		private Response BeforeRequest(ILifetimeScope container, NancyContext context)
		{
			if(Context.EnableCors && context.Request.Method == "OPTIONS")
			{
				var response = new Response { StatusCode = HttpStatusCode.OK };

				AddCorsHeaders(response);

				return response;
			}

			SetCallItem(container, context);

			return null;
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

      if(container.TryResolve(out views))
      {
        context.Items[WebApi.ViewsItemKey] = views;
      }

      if(container.TryResolve(out timeline))
			{
				context.Items[WebApi.TimelineItemKey] = timeline;
			}
		}

		private Task AfterRequest(ILifetimeScope container, NancyContext pipelineContext)
		{
			if(Context.EnableCors)
			{
				AddCorsHeaders(pipelineContext.Response);
			}

			return Task.CompletedTask;
		}

		private static void AddCorsHeaders(Response response)
		{
			response
				.WithHeader("Access-Control-Allow-Origin", "*")
				.WithHeader("Access-Control-Allow-Headers", "Authorization, Origin, Content-Type");
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