using System.Text.Json;

namespace Totem.Http.Commands;

public class HttpCommandNegotiator : IHttpCommandNegotiator
{
    readonly JsonSerializerOptions _jsonOptions;

    public HttpCommandNegotiator(JsonSerializerOptions jsonOptions) =>
        _jsonOptions = jsonOptions ?? throw new ArgumentNullException(nameof(jsonOptions));

    public HttpRequestMessage Negotiate(IHttpCommand command, string contentType)
    {
        if(command is null)
            throw new ArgumentNullException(nameof(command));

        if(string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentOutOfRangeException(nameof(contentType));

        if(!ContentTypes.IsJson(contentType))
            throw new InvalidOperationException($"Unsupported command content type: {contentType}");

        var info = HttpCommandInfo.From(command.GetType());
        var method = new HttpMethod(info.Request.Method);
        var route = RouteFormatter.Format(info.Request.Route, command);
        var json = JsonSerializer.Serialize(command, info.DeclaredType, _jsonOptions);

        return new HttpRequestMessage(method, route)
        {
            Content = new StringContent(json, null, contentType)
        };
    }
}
