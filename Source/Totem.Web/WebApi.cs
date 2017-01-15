using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
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
    public static readonly string ClientItemKey = typeof(WebApi).FullName + ".Client";
    public static readonly string ViewsItemKey = typeof(WebApi).FullName + ".Views";
    public static readonly string TimelineItemKey = typeof(WebApi).FullName + ".Timeline";
    public static readonly string LifetimeItemKey = typeof(WebApi).FullName + ".Lifetime";

    WebApiRequest _request;
    RequestType _requestType;

    Tags ITaggable.Tags => Tags;
    protected Tags Tags { get; } = new Tags();
		protected IClock Clock => Notion.Traits.Clock.Get(this);
		protected ILog Log => Notion.Traits.Log.Get(this);
		protected RuntimeMap Runtime => Notion.Traits.Runtime.Get(this);

    public sealed override string ToString() => ToText();
    public virtual Text ToText() => Call.Link.ToText();

    public WebApiCall Call => ReadContextItem<WebApiCall>(CallItemKey);
    protected Client Client => ReadContextItem<Client>(ClientItemKey);
    protected IViewDb Views => ReadContextItem<IViewDb>(ViewsItemKey);
    protected ITimelineScope Timeline => ReadContextItem<ITimelineScope>(TimelineItemKey);
    protected ILifetimeScope Lifetime => ReadContextItem<ILifetimeScope>(LifetimeItemKey);

    protected T ReadContextItem<T>(string key, bool strict = true)
		{
			object item;

      if(Context.Items.TryGetValue(key, out item))
      {
        if(item is T)
        {
          return (T) item;
        }
        
        // TODO: Address this with the future expectation API

        Expect.True<bool>(strict).IsFalse(
          t => t,
          Totem.Text.Of("Unexpected context item type for key \"{0}\"", key),
          Totem.Text.OfType<T>(),
          _ => Totem.Text.OfType(item));
      }
      else
      {
        Expect.False(strict, $"Missing context item: {key}");
      }

      return default(T);
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

    protected void Authorize(bool isAuthorized)
    {
      if(!isAuthorized)
      {
        throw new RequestDeniedException();
      }
    }

    //
    // Reads
    //

    protected void Get<TView>(string path, Func<dynamic, Id> selectViewId = null) where TView : View
    {
      Get(typeof(TView), path, selectViewId);
    }

    protected void Get(Type viewType, string path, Func<dynamic, Id> selectViewId = null)
    {
      Get(Runtime.GetView(viewType), path, selectViewId);
    }

    protected void Get(ViewType viewType, string path, Func<dynamic, Id> selectViewId = null)
    {
      Get(path, async args =>
      {
        var id = selectViewId == null ? Id.Unassigned : selectViewId(args);

        var key = FlowKey.From(viewType, id);

        return await GetView(key);
      });
    }

    async Task<Response> GetView(FlowKey key)
    {
      var view = await ReadJsonView(key);

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

    Task<ViewSnapshot<string>> ReadJsonView(FlowKey key)
    {
      return Views.ReadJsonSnapshot(key, ReadRequestCheckpoint(key));
    }

    TimelinePosition ReadRequestCheckpoint(FlowKey key)
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

    static Response RespondNotFound(ViewSnapshot<string> view)
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

    static Response RespondNotModified(ViewSnapshot<string> view)
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

    static Response RespondOK(ViewSnapshot<string> view)
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

    static string GetETag(ViewSnapshot<string> view)
    {
      return view.Key.ToText().WriteIf(view.Checkpoint.IsSome, $"@{view.Checkpoint.ToInt64OrNull()}");
    }

    //
    // Writes
    //

    protected void Delete(string path, Func<dynamic, WebApiRequest> selectRequest)
    {
      Delete(path, Execute(selectRequest));
    }

    protected void Patch(string path, Func<dynamic, WebApiRequest> selectRequest)
    {
      Patch(path, Execute(selectRequest));
    }

    protected void Post(string path, Func<dynamic, WebApiRequest> selectRequest)
    {
      Post(path, Execute(selectRequest));
    }

    protected void Put(string path, Func<dynamic, WebApiRequest> selectRequest)
    {
      Put(path, Execute(selectRequest));
    }

    protected void Delete<TRequest>(string path) where TRequest : WebApiRequest, new()
    {
      Delete(path, Execute<TRequest>());
    }

    protected void Patch<TRequest>(string path) where TRequest : WebApiRequest, new()
    {
      Patch(path, Execute<TRequest>());
    }

    protected void Post<TRequest>(string path) where TRequest : WebApiRequest, new()
    {
      Post(path, Execute<TRequest>());
    }

    protected void Put<TRequest>(string path) where TRequest : WebApiRequest, new()
    {
      Put(path, Execute<TRequest>());
    }

    //
    // Execution
    //

    Func<dynamic, Task<object>> Execute(Func<dynamic, WebApiRequest> selectRequest)
    {
      return args => Execute(selectRequest(args));
    }

    Func<dynamic, Task<object>> Execute<TRequest>() where TRequest : WebApiRequest, new()
    {
      return args => Execute(new TRequest());
    }

    async Task<object> Execute(WebApiRequest request)
    {
      try
      {
        _request = request;
        _requestType = Runtime.GetRequest(request.GetType());

        BindRequest();

        await ExecuteRequest();

        return _request.Response;
      }
      finally
      {
        _request = null;
        _requestType = null;
      }
    }

    void BindRequest()
    {
      var id = Id.FromGuid();

      var key = FlowKey.From(_requestType, id);

      FlowContext.Bind(_request, key);

      TryBindRequestBody();
    }

    void TryBindRequestBody()
    {
      if(Call.Body.MediaType != MediaType.Json)
      {
        this.BindTo(_request);
      }
      else
      {
        using(var body = Call.Body.ReadAsStream())
        using(var reader = new StreamReader(body))
        {
          var json = reader.ReadToEnd();

          if(json.Length > 0)
          {
            JsonFormat.Text.DeserializeInto(json, _request);
          }
        }
      }
    }

    Task ExecuteRequest()
    {
      return Timeline.Execute(_request, Client);
    }
  }
}