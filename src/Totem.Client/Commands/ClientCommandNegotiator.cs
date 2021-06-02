using System;
using System.Net.Http;
using System.Text.Json;
using Totem.Http;

namespace Totem.Commands
{
    public class ClientCommandNegotiator : IClientCommandNegotiator
    {
        readonly JsonSerializerOptions _jsonOptions;

        public ClientCommandNegotiator(JsonSerializerOptions jsonOptions) =>
            _jsonOptions = jsonOptions ?? throw new ArgumentNullException(nameof(jsonOptions));

        public HttpRequestMessage Negotiate(IHttpCommand command, string contentType)
        {
            if(command == null)
                throw new ArgumentNullException(nameof(command));

            if(string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentOutOfRangeException(nameof(contentType));

            if(!ContentTypes.IsJson(contentType))
                throw new InvalidOperationException($"Unsupported command content type: {contentType}");

            var info = HttpCommandInfo.From(command);
            var method = new HttpMethod(info.Request.Method);
            var route = RouteFormatter.Format(info.Request.Route, command);
            var json = JsonSerializer.Serialize(command, info.MessageType, _jsonOptions);

            return new HttpRequestMessage(method, route)
            {
                Content = new StringContent(json, null, contentType)
            };
        }
    }
}