using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
	public abstract class WebApiApp : AutofacNancyBootstrapper, IWebApp, IBindable
	{
		protected WebApiApp(WebAppContext appContext)
		{
			AppContext = appContext;
		}

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Fields IBindable.Fields => Fields;

    protected Fields Fields { get; } = new Fields();
		protected IClock Clock => Notion.Traits.Clock.Get(this);
		protected ILog Log => Notion.Traits.Log.Get(this);
		protected RuntimeMap Runtime => Notion.Traits.Runtime.Get(this);

		protected readonly WebAppContext AppContext;

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

			foreach(var binding in AppContext.Bindings)
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

		protected override Func<ITypeCatalog, NancyInternalConfiguration> InternalConfiguration
		{
			// Regain control of headers in 404 responses
			get { return NancyInternalConfiguration.WithOverrides(c => c.StatusCodeHandlers.Clear()); }
		}

    //
    // Composition
    //

    protected override ILifetimeScope GetApplicationContainer()
		{
			return AppContext.Scope;
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
				Http.HttpLink.From(context.Request.Url.ToString()),
				HttpAuthorization.From(context.Request.Headers.Authorization),
				WebApiCallBody.From(
          context.Request.Headers.ContentType ?? "",
          () => context.Request.Body)))
			.InstancePerRequest();

			module.Update(container.ComponentRegistry);
		}

		//
		// Request lifecycle
		//

		protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
		{
      pipelines.BeforeRequest += (context2, cancel) => BeforeRequest(container, context2);
			pipelines.OnError += (context2, exception) => OnRequestError(container, context2, exception);
			pipelines.AfterRequest += context2 => AfterRequest(container, context2);
		}

		async Task<Response> BeforeRequest(ILifetimeScope container, NancyContext context)
		{
			if(AppContext.EnableCors && context.Request.Method == "OPTIONS")
			{
				var response = new Response { StatusCode = HttpStatusCode.OK };

				AddCorsHeaders(response);

				return response;
			}

			await SetCallItems(container, context);

			return null;
		}

    static void AddCorsHeaders(Response response)
    {
      response
        .WithHeader("Access-Control-Allow-Origin", "*")
        .WithHeader("Access-Control-Allow-Headers", "Authorization, Origin, Content-Type");
    }

    static async Task SetCallItems(ILifetimeScope container, NancyContext context)
    {
      context.Items[WebApi.LifetimeItemKey] = container;

      IViewDb views;
      ITimelineScope timeline;

      if(container.TryResolve(out views))
      {
        context.Items[WebApi.ViewsItemKey] = views;
      }

      if(container.TryResolve(out timeline))
      {
        context.Items[WebApi.TimelineItemKey] = timeline;
      }

      var call = container.Resolve<WebApiCall>();

      context.Items[WebApi.CallItemKey] = call;

      IWebUserDb userAuthority;

      var user = container.TryResolve(out userAuthority)
        ? await userAuthority.Authenticate(call.Authorization)
        : new User();

      context.Items[WebApi.UserItemKey] = user;

      context.CurrentUser = user.Principal;
    }

    Response OnRequestError(ILifetimeScope container, NancyContext context, Exception exception)
    {
      var response = container.Resolve<IErrorHandler>().CreateResponse(context, exception);

      if(((int) response.StatusCode) >= 500)
      {
        Log.Error(exception, "[web] Request error in {Url:l}", context.Request.Url);
      }

      return response;
    }

    Task AfterRequest(ILifetimeScope container, NancyContext context)
		{
			if(AppContext.EnableCors)
			{
				AddCorsHeaders(context.Response);
			}

      return Task.CompletedTask;
		}

    sealed class LogAdapter : Notion, ILoggerFactory, ILogger
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

			static LogLevel GetLevel(TraceEventType type)
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