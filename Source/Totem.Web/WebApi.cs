using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json.Linq;
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
    public static readonly string UserItemKey = typeof(WebApi).FullName + ".User";
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
    protected User User => ReadContextItem<User>(UserItemKey);
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

    //
    // Reads
    //

    protected void Get<TView>(string path, Func<bool> authorize = null) where TView : View
    {
      Get(typeof(TView), path, authorize);
    }

    protected void Get(Type viewType, string path, Func<bool> authorize = null)
    {
      var runtimeType = Runtime.GetView(viewType);

      Get(path, args =>
      {
        if(authorize != null && !authorize())
        {
          throw new UnauthorizedAccessException();
        }

        return GetView(FlowKey.From(runtimeType));
      });
    }

    protected void Get<TView>(string path, Func<dynamic, string> selectId, Func<Id, bool> authorize = null) where TView : View
    {
      Get(typeof(TView), path, selectId, authorize);
    }

    protected void Get(Type viewType, string path, Func<dynamic, string> selectId, Func<Id, bool> authorize = null)
    {
      var runtimeType = Runtime.GetView(viewType);

      Get(path, async args =>
      {
        var id = Id.From(selectId(args));

        if(authorize != null && !authorize(id))
        {
          throw new UnauthorizedAccessException();
        }

        return await GetView(FlowKey.From(runtimeType, id));
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
        ContentType = MediaType.Json.ToTextUtf8()
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
        ContentType = MediaType.Json.ToTextUtf8()
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
        ContentType = MediaType.Json.ToTextUtf8(),
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

    protected void Delete<TRequest>(string path, Action<TRequest> init) where TRequest : WebApiRequest, new()
    {
      Delete(path, Execute(init));
    }

    protected void Patch<TRequest>(string path, Action<TRequest> init) where TRequest : WebApiRequest, new()
    {
      Patch(path, Execute(init));
    }

    protected void Post<TRequest>(string path, Action<TRequest> init) where TRequest : WebApiRequest, new()
    {
      Post(path, Execute(init));
    }

    protected void Put<TRequest>(string path, Action<TRequest> init) where TRequest : WebApiRequest, new()
    {
      Put(path, Execute(init));
    }

    //
    // Execution
    //

    Func<dynamic, Task<object>> Execute<TRequest>() where TRequest : WebApiRequest, new()
    {
      return args => Execute(new TRequest());
    }

    Func<dynamic, Task<object>> Execute<TRequest>(Action<TRequest> init) where TRequest : WebApiRequest, new()
    {
      return args =>
      {
        var request = new TRequest();

        init(request);

        return Execute(request);
      };
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

      new ContentBinding(this).Bind();
    }

    Task ExecuteRequest()
    {
      return Timeline.Execute(_request, User);
    }

    class ContentBinding
    {
      readonly WebApi _api;
      readonly WebApiRequest _request;
      readonly IDictionary<string, dynamic> _args;
      readonly bool _anyArgs;

      internal ContentBinding(WebApi api)
      {
        _api = api;
        _request = api._request;

        _args = _api.Context.Parameters as IDictionary<string, dynamic>;

        _anyArgs = _args?.Count > 0;
      }

      internal void Bind()
      {
        if(IsNonJson)
        {
          BindNonJson();
        }
        else if(CanReadBody)
        {
          BindJson();
        }
        else
        {
          BindArgs();
        }
      }

      bool IsNonJson => _api.Call.Body.MediaType != MediaType.Json;
      bool CanReadBody => _api.Call.Body.CanRead;

      void BindNonJson()
      {
        if(CanReadBody)
        {
          _api.BindTo(_request);
        }
        else
        {
          BindArgs();
        }
      }

      void BindJson()
      {
        // See Totem.Web.WebArea.cs for an explanation of why we can't use BindTo for JSON :-(

        var json = ReadJson();

        if(!_anyArgs)
        {
          if(json.Length > 0)
          {
            BindRequest(json);
          }
        }
        else if(json.Length > 0)
        {
          BindRequest(JsonFormat.Text.DeserializeJson(json));
        }
        else
        {
          BindArgs();
        }
      }

      string ReadJson()
      {
        using(var body = _api.Call.Body.ReadAsStream())
        using(var reader = new StreamReader(body))
        {
          return reader.ReadToEnd();
        }
      }

      void BindRequest(string json)
      {
        JsonFormat.Text.DeserializeInto(json, _request);
      }

      void BindRequest(JObject binding)
      {
        foreach(var arg in _args)
        {
          // If the route and the body both contain the same value, but with different cases,
          // this will create a second property on the object. It will be added after the first,
          // though, so still takes effect via last-in-wins.
          //
          // The other option is to always bind the args separately from the body. This avoids the
          // casing issue entirely, but is more costly to the happy path.
          //
          // As it should be exceedingly rare for a request value to be in both the route and the body,
          // and last-in-wins ensures we'll get the correct value if it does happen, I opted to take
          // the performance boost and limit the binding to one deserialization call.

          binding[arg.Key] = arg.Value.Value;
        }

        BindRequest(binding.ToString());
      }

      void BindArgs()
      {
        BindRequest(new JObject());
      }
    }
  }
}