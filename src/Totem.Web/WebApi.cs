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
	/// A Nancy module representing an instance of a Totem web API bound to an HTTP request
	/// </summary>
	public abstract class WebApi : NancyModule, IWebApi
	{
		public static readonly string CallItemKey = typeof(WebApi).FullName + ".Call";

		protected WebApi()
		{
			Tags = new Tags();
		}

		Tags ITaggable.Tags { get { return Tags; } }
		protected Tags Tags { get; private set; }
		protected IClock Clock { get { return Notion.Traits.Clock.Get(this); } }
		protected ILog Log { get { return Notion.Traits.Log.Get(this); } }
		protected RuntimeMap Runtime { get { return Notion.Traits.Runtime.Get(this); } }

		public WebApiCall Call { get { return ReadContextItem<WebApiCall>(CallItemKey); } }
		protected IViewDb Views { get { return Call.Views; } }

		protected T ReadContextItem<T>(string key, bool strict = true)
		{
			object item;

			Expect.That(Context.Items.TryGetValue(key, out item)).IsTrue("Missing context item: " + key);

			if(item is T)
			{
				return (T) item;
			}

			if(strict)
			{
				throw new Exception(Totem.Text.Of(
					"Unexpected context item type for key \"{0}\"; expected {1}, actual is {2}",
					key,
					typeof(T),
					item.GetType()));
			}

			return default(T);
		}

		public sealed override string ToString()
		{
			return ToText();
		}

		public virtual Text ToText()
		{
			return Call.Link.ToText();
		}
	}
}