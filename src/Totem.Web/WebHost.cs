using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Nancy;
using Totem.Http;
using Totem.IO;

namespace Totem.Web
{
	/// <summary>
	/// The context of a set of web applications
	/// </summary>
	public sealed class WebHost : Connection
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

		//
		// API app factory
		//

		public static IWebApp CreateApp(WebAppContext context, IEnumerable<Type> apiTypes)
		{
			return new ApiApp(context, apiTypes.ToList());
		}

		public static IWebApp CreateApp(WebAppContext context, params Type[] apiTypes)
		{
			return new ApiApp(context, apiTypes);
		}

		public static IWebApp CreateApp<TApi>(WebAppContext context)
		{
			return CreateApp(context, typeof(TApi));
		}

		public static IWebApp CreateApp<TApi1, TApi2>(WebAppContext context)
		{
			return CreateApp(context, typeof(TApi1), typeof(TApi2));
		}

		public static IWebApp CreateApp<TApi1, TApi2, TApi3>(WebAppContext context)
		{
			return CreateApp(context, typeof(TApi1), typeof(TApi2), typeof(TApi3));
		}

		public static IWebApp CreateApp<TApi1, TApi2, TApi3, TApi4>(WebAppContext context)
		{
			return CreateApp(context, typeof(TApi1), typeof(TApi2), typeof(TApi3));
		}

		public static IWebApp CreateApp(IReadOnlyList<HttpLink> bindings, ILifetimeScope scope, IEnumerable<Type> apiTypes)
		{
			return CreateApp(new WebAppContext(bindings, scope), apiTypes);
		}

		public static IWebApp CreateApp(IReadOnlyList<HttpLink> bindings, ILifetimeScope scope, params Type[] apiTypes)
		{
			return CreateApp(new WebAppContext(bindings, scope), apiTypes);
		}

		public static IWebApp CreateApp<TApi>(IReadOnlyList<HttpLink> bindings, ILifetimeScope scope)
		{
			return CreateApp<TApi>(new WebAppContext(bindings, scope));
		}

		public static IWebApp CreateApp<TApi1, TApi2>(IReadOnlyList<HttpLink> bindings, ILifetimeScope scope)
		{
			return CreateApp<TApi1, TApi2>(new WebAppContext(bindings, scope));
		}

		public static IWebApp CreateApp<TApi1, TApi2, TApi3>(IReadOnlyList<HttpLink> bindings, ILifetimeScope scope)
		{
			return CreateApp<TApi1, TApi2, TApi3>(new WebAppContext(bindings, scope));
		}

		public static IWebApp CreateApp<TApi1, TApi2, TApi3, TApi4>(IReadOnlyList<HttpLink> bindings, ILifetimeScope scope)
		{
			return CreateApp<TApi1, TApi2, TApi3, TApi4>(new WebAppContext(bindings, scope));
		}

		private sealed class ApiApp : WebApiApp
		{
			private readonly IReadOnlyList<Type> _apiTypes;

			internal ApiApp(WebAppContext context, IReadOnlyList<Type> apiTypes) : base(context)
			{
				_apiTypes = apiTypes;
			}

			protected override IEnumerable<INancyModule> GetAllModules(ILifetimeScope container)
			{
				return _apiTypes.Select(apiType => (INancyModule) container.Resolve(apiType));
			}
		}

		//
		// UI app factory
		//

		public static IWebApp CreateUIApp(WebUIContext context, IEnumerable<Type> apiTypes)
		{
			return new UIApp(context, apiTypes.ToList());
		}

		public static IWebApp CreateUIApp(WebUIContext context, params Type[] apiTypes)
		{
			return new UIApp(context, apiTypes);
		}

		public static IWebApp CreateUIApp<TApi>(WebUIContext context)
		{
			return CreateUIApp(context, typeof(TApi));
		}

		public static IWebApp CreateUIApp<TApi1, TApi2>(WebUIContext context)
		{
			return CreateUIApp(context, typeof(TApi1), typeof(TApi2));
		}

		public static IWebApp CreateUIApp<TApi1, TApi2, TApi3>(WebUIContext context)
		{
			return CreateUIApp(context, typeof(TApi1), typeof(TApi2), typeof(TApi3));
		}

		public static IWebApp CreateUIApp<TApi1, TApi2, TApi3, TApi4>(WebUIContext context)
		{
			return CreateUIApp(context, typeof(TApi1), typeof(TApi2), typeof(TApi3));
		}

		public static IWebApp CreateUIApp(IReadOnlyList<HttpLink> bindings, ILifetimeScope scope, FolderLink contentFolder, IEnumerable<Type> apiTypes)
		{
			return CreateUIApp(new WebUIContext(bindings, scope, contentFolder), apiTypes);
		}

		public static IWebApp CreateUIApp(IReadOnlyList<HttpLink> bindings, ILifetimeScope scope, FolderLink contentFolder, params Type[] apiTypes)
		{
			return CreateUIApp(new WebUIContext(bindings, scope, contentFolder), apiTypes);
		}

		public static IWebApp CreateUIApp<TApi>(IReadOnlyList<HttpLink> bindings, ILifetimeScope scope, FolderLink contentFolder)
		{
			return CreateUIApp<TApi>(new WebUIContext(bindings, scope, contentFolder));
		}

		public static IWebApp CreateUIApp<TApi1, TApi2>(IReadOnlyList<HttpLink> bindings, ILifetimeScope scope, FolderLink contentFolder)
		{
			return CreateUIApp<TApi1, TApi2>(new WebUIContext(bindings, scope, contentFolder));
		}

		public static IWebApp CreateUIApp<TApi1, TApi2, TApi3>(IReadOnlyList<HttpLink> bindings, ILifetimeScope scope, FolderLink contentFolder)
		{
			return CreateUIApp<TApi1, TApi2, TApi3>(new WebUIContext(bindings, scope, contentFolder));
		}

		public static IWebApp CreateUIApp<TApi1, TApi2, TApi3, TApi4>(IReadOnlyList<HttpLink> bindings, ILifetimeScope scope, FolderLink contentFolder)
		{
			return CreateUIApp<TApi1, TApi2, TApi3, TApi4>(new WebUIContext(bindings, scope, contentFolder));
		}

		private sealed class UIApp : WebUIApp
		{
			private readonly IReadOnlyList<Type> _apiTypes;

			internal UIApp(WebUIContext context, IReadOnlyList<Type> apiTypes) : base(context)
			{
				_apiTypes = apiTypes;
			}

			protected override IEnumerable<INancyModule> GetAllModules(ILifetimeScope container)
			{
				return _apiTypes.Select(apiType => (INancyModule) container.Resolve(apiType));
			}
		}
	}
}