using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Http;

public class RouteFormatter
{
    static readonly ConcurrentDictionary<Type, RouteFormatterType> _formattersByType = new();

    public RouteFormatter(string template, object message)
    {
        if(string.IsNullOrWhiteSpace(template))
            throw new ArgumentOutOfRangeException(nameof(template));

        if(message is null)
            throw new ArgumentNullException(nameof(message));

        if(!template.Contains("{", StringComparison.Ordinal))
        {
            Route = template;
            Tokens = Enumerable.Empty<string>();
        }
        else
        {
            var formatter = _formattersByType.GetOrAdd(message.GetType(), type => new RouteFormatterType(template, type));

            Route = formatter.Format(message);
            Tokens = formatter.Tokens;
        }
    }

    public string Route { get; }
    public IEnumerable<string> Tokens { get; }

    public static string Format(string route, object message) =>
        new RouteFormatter(route, message).Route;
}
