using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Nancy;
using Nancy.Conventions;
using Totem.IO;
using Totem.Runtime;

namespace Totem.Web
{
	/// <summary>
	/// An HTTP-bound API serving a user interface
	/// </summary>
	public abstract class WebUIApp : WebApiApp, IRootPathProvider
	{
		protected WebUIApp(WebUIContext context) : base(context)
		{}

		public new WebUIContext Context
		{
			get { return (WebUIContext) base.Context; }
		}

		protected override IRootPathProvider RootPathProvider
		{
			get { return this; }
		}

		public string GetRootPath()
		{
			return Context.ContentFolder.ToString();
		}

		protected override void ConfigureConventions(NancyConventions nancyConventions)
		{
			base.ConfigureConventions(nancyConventions);

			new UIConventions(this, nancyConventions).Configure();
		}

		private sealed class UIConventions
		{
			private readonly WebUIApp _app;
			private readonly NancyConventions _conventions;

			internal UIConventions(WebUIApp app, NancyConventions conventions)
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
	}
}