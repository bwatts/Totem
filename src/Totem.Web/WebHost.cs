using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin.Builder;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Logging;
using Owin;
using Totem.Http;

namespace Totem.Web
{
	/// <summary>
	/// The context of an instance of a web application
	/// </summary>
	public sealed class WebHost : Connection
	{
		private readonly IReadOnlyList<IWebApp> _apps;

		public WebHost(IReadOnlyList<IWebApp> apps)
		{
			_apps = apps;
		}

		protected override void Open()
		{
			foreach(var app in _apps)
			{
				Start(app);
			}
		}

		private void Start(IWebApp app)
		{
			Track(WebApp.Start(GetStartOptions(app), GetStartup(app)));
		}

		private static StartOptions GetStartOptions(IWebApp app)
		{
			var options = new StartOptions();

			foreach(var hostBinding in app.HostBindings)
			{
				options.Urls.Add(hostBinding.ToString());
			}

			return options;
		}

		private static Action<IAppBuilder> GetStartup(IWebApp app)
		{
			return builder =>
			{
				builder.SetLoggerFactory(new LogAdapter(app));

				app.Start(builder);
			};
		}

		private sealed class LogAdapter : Notion, ILoggerFactory, ILogger
		{
			private readonly IWebApp _app;

			internal LogAdapter(IWebApp app)
			{
				_app = app;
			}

			public ILogger Create(string name)
			{
				return this;
			}

			public bool WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
			{
				var level = GetLevel(eventType);

				// OwinHttpListener seems to always throw ObjectDisposedException when shutting down

				var canWrite = Log.IsAt(level) && !(exception is ObjectDisposedException);

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