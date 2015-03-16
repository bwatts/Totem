using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Totem.Http;
using Totem.Runtime;
using Totem.Runtime.Map;

namespace Totem.Web
{
	/// <summary>
	/// A Nancy module representing part of the Totem web API
	/// </summary>
	public abstract class WebApi : NancyModule, IWebApi
	{
		public static readonly string ScopeItemKey = typeof(WebApi).FullName + ".Scope";

		protected WebApi()
		{
			Tags = new Tags();
		}

		Tags ITaggable.Tags { get { return Tags; } }
		protected Tags Tags { get; private set; }
		protected IClock Clock { get { return Notion.Traits.Clock.Get(this); } }
		protected ILog Log { get { return Notion.Traits.Log.Get(this); } }
		protected RuntimeMap Runtime { get { return Notion.Traits.Runtime.Get(this); } }

		WebApiScope IWebApi.Scope { get { return Scope; } }
		HttpLink IWebApi.Link { get { return Link; } }
		HttpAuthorization IWebApi.Authorization { get { return Authorization; } }
		IRequestBody IWebApi.RequestBody { get { return RequestBody; } }
		
		protected WebApiScope Scope
		{
			get
			{
				object item;

				Expect.That(Context.Items.TryGetValue(ScopeItemKey, out item)).IsTrue("Missing context item: " + ScopeItemKey.ToText());

				var scope = item as WebApiScope;

				Expect.That(scope == null).IsFalse(Totem.Text
					.Of("Unexpected context item type ")
					.Write(ScopeItemKey)
					.Write("; expected ")
					.Write(typeof(WebApiScope))
					.Write(", actual is {0}", item.GetType()));

				return scope;
			}
		}

		protected HttpLink Link { get { return Scope.Link; } }
		protected HttpAuthorization Authorization { get { return Scope.Authorization; } }
		protected IRequestBody RequestBody { get { return Scope.RequestBody; } }

		public sealed override string ToString()
		{
			return ToText();
		}

		public virtual Text ToText()
		{
			return Link.ToText();
		}
	}
}