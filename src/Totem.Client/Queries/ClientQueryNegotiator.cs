using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Text.Json;
using Totem.Http;
using Totem.Queries.Encoding;

namespace Totem.Queries
{
    public class ClientQueryNegotiator : IClientQueryNegotiator
    {
        readonly ConcurrentDictionary<Type, QueryEncoder> _encodersByQueryType = new();
        readonly JsonSerializerOptions _jsonOptions;

        public ClientQueryNegotiator(JsonSerializerOptions jsonOptions) =>
            _jsonOptions = jsonOptions ?? throw new ArgumentNullException(nameof(jsonOptions));

        public HttpRequestMessage Negotiate(IQuery query)
        {
            if(query == null)
                throw new ArgumentNullException(nameof(query));

            var info = QueryInfo.From(query.GetType());
            var method = new HttpMethod(info.Method);
            var route = EncodeRoute(info.Route, query);

            return new HttpRequestMessage(method, route);
        }

        public object? NegotiateResult(Type resultType, string contentType, string content)
        {
            if(ContentTypes.IsPlainText(contentType))
            {
                return content;
            }

            if(ContentTypes.IsJson(contentType))
            {
                return JsonSerializer.Deserialize(content, resultType, _jsonOptions);
            }

            throw new InvalidOperationException($"Unsupported query content type: {contentType}");
        }

        Uri EncodeRoute(string template, IQuery query)
        {
            var formatter = new RouteFormatter(template, query);
            var encoder = _encodersByQueryType.GetOrAdd(query.GetType(), type => new QueryEncoder(type, formatter.Tokens));

            var queryString = encoder.Encode(query);
            var uri = queryString.Length == 0 ? formatter.Route : $"{formatter.Route}?{queryString}";

            return new Uri(uri, UriKind.Relative);
        }
    }
}