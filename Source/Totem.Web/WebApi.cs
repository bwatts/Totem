using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Totem.IO;
using Totem.Runtime;
using Totem.Runtime.Json;
using Totem.Runtime.Map;
using Totem.Runtime.Map.Timeline;
using Totem.Runtime.Timeline;

namespace Totem.Web
{
	/// <summary>
	/// A Nancy module representing an instance of a Totem web API bound to an HTTP request
	/// </summary>
	public abstract class WebApi : NancyModule, IWebApi
	{
		public static readonly string CallItemKey = typeof(WebApi).FullName + ".Call";
		public static readonly string ViewsItemKey = typeof(WebApi).FullName + ".Views";
		public static readonly string TimelineItemKey = typeof(WebApi).FullName + ".Timeline";

		protected WebApi()
		{
			Tags = new Tags();
		}

		Tags ITaggable.Tags => Tags;
		protected Tags Tags { get; private set; }
		protected IClock Clock => Notion.Traits.Clock.Get(this);
		protected ILog Log => Notion.Traits.Log.Get(this);
		protected RuntimeMap Runtime => Notion.Traits.Runtime.Get(this);

		public WebApiCall Call => ReadContextItem<WebApiCall>(CallItemKey);
		protected IViewDb Views => ReadContextItem<IViewDb>(ViewsItemKey);
		protected ITimeline Timeline => ReadContextItem<ITimeline>(TimelineItemKey);

		public sealed override string ToString() => ToText();
		public virtual Text ToText() => Call.Link.ToText();

		protected T ReadContextItem<T>(string key, bool strict = true)
		{
			object item;

			Expect.True(Context.Items.TryGetValue(key, out item), "Missing context item: " + key);

			if(item is T)
			{
				return (T) item;
			}

			Expect.True<bool>(strict).IsFalse(
				t => t,
				Totem.Text.Of("Unexpected context item type for key \"{0}\"", key),
				Totem.Text.OfType<T>(),
				_ => Totem.Text.OfType(item));

			return default(T);
		}

		protected Response MakeRequest<T>(Event e) where T : WebApiRequest
		{
      // This could be async all the way back to the API classes...but it doesn't read nearly as well there :-)

      // TODO: Look into effects of blocking threads here

      var flow = Timeline.MakeRequest<T>(e).Result;

      return flow.ToResponse();
		}

		//
		// Body
		//

		protected T ReadBody<T>() where T : class
		{
			// See Totem.Web.WebArea.cs for an explanation; it could be argued that this is fairly hacky.

			if(Call.Body.MediaType == MediaType.Json)
			{
				using(var body = Call.Body.ReadAsStream())
				using(var reader = new StreamReader(body))
				{
					return JsonFormat.Text.Deserialize<T>(reader.ReadToEnd());
				}
			}

			return this.Bind<T>();
		}

    //
    // Views
    //

    protected View ReadView(Type type, Id id, bool strict = true)
    {
      return Views.Read(type, id, strict);
    }

    protected TView ReadView<TView>(Id id, bool strict = true) where TView : View
    {
      return Views.Read<TView>(id, strict);
    }

    protected View ReadView(ViewType type, Id id, bool strict = true)
    {
      return Views.Read(type, id, strict);
    }

    protected View ReadView(Type type, bool strict = true)
    {
      return Views.Read(type, strict);
    }

    protected TView ReadView<TView>(bool strict = true) where TView : View
    {
      return Views.Read<TView>(strict);
    }

    protected View ReadView(ViewType type, bool strict = true)
    {
      return Views.Read(type, strict);
    }

    //
    // Views (JSON)
    //

    protected string ReadViewJson(Type type, Id id, bool strict = true)
    {
      return Views.ReadJson(type, id, strict);
    }

    protected string ReadViewJson<TView>(Id id, bool strict = true) where TView : View
    {
      return Views.ReadJson<TView>(id, strict);
    }

    protected string ReadViewJson(ViewType type, Id id, bool strict = true)
    {
      return Views.ReadJson(type, id, strict);
    }

    protected string ReadViewJson(Type type, bool strict = true)
    {
      return Views.ReadJson(type, strict);
    }

    protected string ReadViewJson<TView>(bool strict = true) where TView : View
    {
      return Views.ReadJson<TView>(strict);
    }

    protected string ReadViewJson(ViewType type, bool strict = true)
    {
      return Views.ReadJson(type, strict);
    }

    //
    // Views (GET)
    //

    protected dynamic GetView(Type type, Id id)
    {
      return GetView(Runtime.GetView(type), id);
    }

    protected dynamic GetView<T>(Id id) where T : View
    {
      return GetView(typeof(T), id);
    }

    protected dynamic GetView(ViewType type, Id id)
    {
      var view = ReadView(type, id, strict: false);

      if(view != null)
      {
        return view;
      }

      return new Response
      {
        StatusCode = HttpStatusCode.NotFound,
        ReasonPhrase = "Failed to read " + type.ToText().WriteIf(id.IsAssigned, $"/{id}")
      };
    }

    protected dynamic GetView(Type type)
    {
      return GetView(Runtime.GetView(type));
    }

    protected dynamic GetView<T>() where T : View
    {
      return GetView(typeof(T));
    }

    protected dynamic GetView(ViewType type)
    {
      var view = ReadView(type, strict: false);

      if(view != null)
      {
        return view;
      }

      return new Response
      {
        StatusCode = HttpStatusCode.NotFound,
        ReasonPhrase = "Failed to read " + type.ToString()
      };
    }

    //
    // Views (GET JSON)
    //

    protected dynamic GetViewJson(Type type, Id id)
    {
      return GetViewJson(Runtime.GetView(type), id);
    }

    protected dynamic GetViewJson<T>(Id id) where T : View
    {
      return GetViewJson(typeof(T), id);
    }

    protected dynamic GetViewJson(ViewType type, Id id)
    {
      var json = ReadViewJson(type, id, strict: false);

      if(json != null)
      {
        return new Response
        {
          StatusCode = HttpStatusCode.OK,
          ContentType = MediaType.Json.ToString(),
          Contents = body =>
          {
            using(var writer = new StreamWriter(body))
            {
              writer.Write(json);
            }
          }
        };
      }

      return new Response
      {
        StatusCode = HttpStatusCode.NotFound,
        ReasonPhrase = "Failed to read " + type.ToText().WriteIf(id.IsAssigned, $"/{id}")
      };
    }

    protected dynamic GetViewJson(Type type)
    {
      return GetViewJson(Runtime.GetView(type));
    }

    protected dynamic GetViewJson<T>() where T : View
    {
      return GetViewJson(typeof(T));
    }

    protected dynamic GetViewJson(ViewType type)
    {
      var json = ReadViewJson(type, strict: false);

      if(json != null)
      {
        return new Response
        {
          StatusCode = HttpStatusCode.OK,
          ContentType = MediaType.Json.ToString(),
          Contents = body =>
          {
            using(var writer = new StreamWriter(body))
            {
              writer.Write(json);
            }
          }
        };
      }

      return new Response
      {
        StatusCode = HttpStatusCode.NotFound,
        ReasonPhrase = "Failed to read " + type.ToString()
      };
    }
  }
}