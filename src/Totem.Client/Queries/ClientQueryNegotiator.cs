using System.Collections.Concurrent;
using System.Text.Json;
using Totem.Http;
using Totem.Queries.Encoding;

namespace Totem.Queries;

public class ClientQueryNegotiator : IClientQueryNegotiator
{
    readonly ConcurrentDictionary<Type, QueryEncoder> _encodersByQueryType = new();
    readonly JsonSerializerOptions _jsonOptions;

    public ClientQueryNegotiator(JsonSerializerOptions jsonOptions) =>
        _jsonOptions = jsonOptions ?? throw new ArgumentNullException(nameof(jsonOptions));

    public HttpRequestMessage Negotiate(IHttpQuery query)
    {
        if(query is null)
            throw new ArgumentNullException(nameof(query));

        var info = HttpQueryInfo.From(query.GetType());

        var method = new HttpMethod(info.Request.Method);
        var route = EncodeRoute(info.Request.Route, query);

        return new HttpRequestMessage(method, route);
    }

    public void NegotiateResult(IClientQueryContext<IHttpQuery> context)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        if(context.Response is null)
            throw new InvalidOperationException("Expected context to have a resposne");

        var contentType = context.Response.ContentType;
        var content = context.Response.Content;

        if(ContentTypes.IsPlainText(contentType))
        {
            context.Result = content;
        }
        else if(ContentTypes.IsJson(contentType))
        {
            context.Result = JsonSerializer.Deserialize(content, context.ResultType, _jsonOptions);
        }
        else
        {
            throw new InvalidOperationException($"Unsupported query content type: {contentType}");
        }
    }

    Uri EncodeRoute(string template, IHttpQuery query)
    {
        var formatter = new RouteFormatter(template, query);
        var encoder = _encodersByQueryType.GetOrAdd(query.GetType(), type => new QueryEncoder(type, formatter.Tokens));

        var queryString = encoder.Encode(query);
        var uri = queryString.Length == 0 ? formatter.Route : $"{formatter.Route}?{queryString}";

        return new Uri(uri, UriKind.Relative);
    }
}
