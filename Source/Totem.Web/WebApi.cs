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
using Totem.Web.Push;

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

		protected dynamic GetView(Type type, Id id)
    {
      return GetView(Runtime.GetView(type), id);
    }

		protected dynamic GetView(Type type)
		{
			return GetView(type, Id.Unassigned);
		}

		protected dynamic GetView<T>(Id id) where T : View
    {
      return GetView(typeof(T), id);
    }

		protected dynamic GetView<T>() where T : View
		{
			return GetView(typeof(T));
		}

		protected dynamic GetView(ViewType type)
		{
			return GetView(FlowKey.From(type));
		}

		protected dynamic GetView(ViewType type, Id id)
		{
			return GetView(FlowKey.From(type, id));
		}

		protected dynamic GetView(FlowKey key)
		{
			var view = ReadJsonView(key);

			if(view.NotFound)
			{
				return RespondNotFound(view);
			}
			else if(view.NotModified)
			{
				return RespondNotModified(view);
			}
			else
			{
				return RespondOK(view);
			}
		}

		private ViewSnapshot<string> ReadJsonView(FlowKey key)
		{
			return Views.ReadJsonSnapshot(key, ReadRequestCheckpoint(key));
		}

		private TimelinePosition ReadRequestCheckpoint(FlowKey key)
		{
			var etag = Request.Headers["If-None-Match"].FirstOrDefault();

			if(!string.IsNullOrWhiteSpace(etag))
			{
				var checkpoint = ViewETag.From(etag, strict: false);

				if(checkpoint != null && checkpoint.Key == key)
				{
					return checkpoint.Checkpoint;
				}
			}

			return TimelinePosition.None;
		}

		private static Response RespondNotFound(ViewSnapshot<string> view)
		{
			return new Response
			{
				StatusCode = HttpStatusCode.NotFound,
#if DEBUG
				ReasonPhrase = $"View not found: {view.Key}",
#endif
				Headers = new Dictionary<string, string>
				{
					["ETag"] = view.Key.ToString()
				},
				ContentType = MediaType.Json.ToString()
			};
		}

		private static Response RespondNotModified(ViewSnapshot<string> view)
		{
			return new Response
			{
				StatusCode = HttpStatusCode.NotModified,
#if DEBUG
				ReasonPhrase = $"View not modified: {view.Key}",
#endif
				Headers = new Dictionary<string, string>
				{
					["ETag"] = GetETag(view)
				},
				ContentType = MediaType.Json.ToString()
			};
		}

		private static Response RespondOK(ViewSnapshot<string> view)
		{
			return new Response
			{
				StatusCode = HttpStatusCode.OK,
#if DEBUG
				ReasonPhrase = $"View found: {view.Key}",
#endif
				Headers = new Dictionary<string, string>
				{
					["ETag"] = GetETag(view)
				},
				ContentType = MediaType.Json.ToString(),
				Contents = body =>
				{
					using(var writer = new StreamWriter(body))
					{
						writer.Write(view.ReadContent());
					}
				}
			};
		}

		private static string GetETag(ViewSnapshot<string> view)
		{
			return view.Key.ToText().WriteIf(view.Checkpoint.IsSome, $"@{view.Checkpoint.ToInt64()}");
    }
	}
}