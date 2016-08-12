using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.Conventions;

namespace Totem.Web
{
	/// <summary>
	/// An HTTP-bound API serving a user interface
	/// </summary>
	public abstract class WebUIApp : WebApiApp, IRootPathProvider
	{
		protected WebUIApp(WebUIContext context) : base(context)
		{}

		public new WebUIContext Context => (WebUIContext) base.Context;

		protected override IRootPathProvider RootPathProvider => this;

		public string GetRootPath() => Context.ContentFolder.ToString();

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
				LocateViews();

				CoerceAcceptHeaders();

				ServeStaticContent();
			}

			private void LocateViews()
			{
				_conventions.ViewLocationConventions.Replace((viewName, model, context) => "dist/" + viewName);
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
				_conventions.StaticContentsConventions.Clear();

				ServeDistContent("images");
				ServeDistContent("css");
				ServeDistContent("js");
				ServeDistContent("fonts");
			}

			private void ServeDistContent(string path)
			{
				_conventions
					.StaticContentsConventions
					.Add(StaticContentConventionBuilder.AddDirectory(path, "dist/" + path));
			}
		}
	}
}