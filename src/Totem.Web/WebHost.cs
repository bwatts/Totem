using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin.Builder;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Logging;
using Totem.Http;

namespace Totem.Web
{
	/// <summary>
	/// The context of an instance of a web application
	/// </summary>
	public sealed class WebHost : Connection
	{
		private readonly HttpLink _binding;
		private readonly IReadOnlyList<IWebApplication> _applications;

		public WebHost(HttpLink binding, IReadOnlyList<IWebApplication> applications)
		{
			_binding = binding;
			_applications = applications;
		}

		protected override void Open()
		{
			foreach(var application in _applications)
			{
				Start(application);
			}
		}

		private void Start(IWebApplication application)
		{
			Track(WebApp.Start(GetUrl(application), builder =>
			{
				builder.SetLoggerFactory(new LogAdapter(application));

				application.Start(builder);
			}));
		}

		private string GetUrl(IWebApplication application)
		{
			return _binding.Then(application.Resource).ToString();
		}

		private sealed class LogAdapter : Notion, ILoggerFactory, ILogger
		{
			private readonly IWebApplication _application;

			internal LogAdapter(IWebApplication application)
			{
				_application = application;
			}

			public ILogger Create(string name)
			{
				return this;
			}

			public bool WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
			{
				var level = GetLevel(eventType);

				// OwinHttpListener always seems to throw ObjectDisposedException when shutting down

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
					case TraceEventType.Information:
						return LogLevel.Verbose;
					case TraceEventType.Warning:
						return LogLevel.Warning;
					case TraceEventType.Error:
						return LogLevel.Error;
					case TraceEventType.Critical:
						return LogLevel.Fatal;
					default:
						return LogLevel.Info;
				}
			}
		}
	}
}