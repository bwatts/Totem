using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Lifetime;
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

namespace Totem.Web
{
	/// <summary>
	/// An HTTP-bound API composed by OWIN and Nancy
	/// </summary>
	public class ApiApplication : AutofacNancyBootstrapper, IWebApplication, IWritable, ITaggable
	{
		private readonly CompositionRoot _compositionRoot;
		private readonly NancyOptions _options;
		private readonly ApiTypeSet _apiTypes;
		private readonly AreaType _area;

		public ApiApplication(CompositionRoot compositionRoot, NancyOptions options, HttpResource resource, ApiTypeSet apiTypes, AreaType area)
		{
			_compositionRoot = compositionRoot;
			_options = options;
			Resource = resource;
			_apiTypes = apiTypes;
			_area = area;
		}

		Tags ITaggable.Tags { get { return Tags; } }
		protected Tags Tags { get; private set; }
		protected IClock Clock { get { return Notion.Traits.Clock.Get(this); } }
		protected ILog Log { get { return Notion.Traits.Log.Get(this); } }
		protected RuntimeMap Runtime { get { return Notion.Traits.Runtime.Get(this); } }

		public HttpResource Resource { get; private set; }

		public Text ToText()
		{
			return Resource.ToText();
		}

		public virtual void Start(IAppBuilder builder)
		{
			_options.Bootstrapper = this;

			builder.UseNancy(_options);
		}

		//
		// Composition
		//

		protected override ILifetimeScope GetApplicationContainer()
		{
			return _compositionRoot.Scope;
		}

		protected override IEnumerable<INancyModule> GetAllModules(ILifetimeScope container)
		{
			return _apiTypes.Select(api => api.Resolve(container));
		}

		protected override INancyModule GetModule(ILifetimeScope container, Type moduleType)
		{
			return (WebApi) container.Resolve(moduleType);
		}

		protected override ILifetimeScope CreateRequestContainer(NancyContext context)
		{
			return _compositionRoot.Scope.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
		}

		protected override void RegisterRequestContainerModules(ILifetimeScope container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
		{}

		protected override void ConfigureRequestContainer(ILifetimeScope container, NancyContext context)
		{
			new RequestModule(context).Update(container.ComponentRegistry);

			base.ConfigureRequestContainer(container, context);
		}

		private sealed class RequestModule : BuilderModule
		{
			public RequestModule(NancyContext context)
			{
				RegisterInstance(context).ExternallyOwned();

				RegisterType<WebApiScope>().InstancePerRequest();

				RegisterType<RequestBody>().As<IRequestBody>().InstancePerRequest();
			}
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
			NormalizeRequest(context);

			SetScopeItem(container, context);

			return context.Response;
		}

		private Response OnRequestError(ILifetimeScope container, NancyContext context, Exception exception)
		{
			return container.Resolve<IErrorHandler>().CreateResponse(context, exception);
		}

		private static void SetScopeItem(ILifetimeScope container, NancyContext context)
		{
			WebApiScope scope;

			if(container.TryResolve(out scope))
			{
				context.Items[WebApi.ScopeItemKey] = scope;
			}
		}

		private void NormalizeRequest(NancyContext context)
		{
			if(context.Request.Url.BasePath != null)
			{
				return;
			}

			// It is unclear why these values are not set properly when we receive the context.
			//
			// In any case, this normalization will ensure that the base path is the area resource, and the request path is relative to it.

			var requestPath = HttpResource.From(context.Request.Url.Path);

			context.Request.Url.BasePath = Resource.ToText(leadingSlash: true);
			context.Request.Url.Path = requestPath.RelativeTo(Resource).ToText(leadingSlash: true);
		}

		//
		// Conventions
		//

		protected override void ConfigureConventions(NancyConventions nancyConventions)
		{
			base.ConfigureConventions(nancyConventions);

			nancyConventions.ViewLocationConventions.Clear();

			nancyConventions.ViewLocationConventions.Add((viewName, model, context) =>
			{
				return "UI/" + viewName;
			});

			nancyConventions.AcceptHeaderCoercionConventions.Add((acceptHeaders, context) =>
			{
				return new[]
				{
					Tuple.Create("text/html", 1.0m),
					Tuple.Create("*/*", 0.9m)
				};
			});

			AddContentFolder(nancyConventions, "css", "UI/css");
			AddContentFolder(nancyConventions, "images", "UI/images");
			AddContentFolder(nancyConventions, "js", "UI/js");
			AddContentFolder(nancyConventions, "references", "UI/references");
		}

		private void AddContentFolder(NancyConventions nancyConventions, string request, string content)
		{
			nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory(
				request,
				_area.Package.Folder.Then(FolderResource.From(content)).ToString()));
		}

		protected override IRootPathProvider RootPathProvider
		{
			get { return new UIPathProvider(_area); }
		}

		private sealed class UIPathProvider : Notion, IRootPathProvider
		{
			private readonly AreaType _area;

			internal UIPathProvider(AreaType area)
			{
				_area = area;
			}

			public string GetRootPath()
			{
				return _area.Package.Folder.ToString();
			}
		}
	}
}