using System.Reflection;
using System.Text.Json.Serialization;

namespace Totem.Queries.Encoding;

internal class QueryEncoder
{
    readonly QueryEncoderProperty[] _properties;

    internal QueryEncoder(Type queryType, IEnumerable<string> routeTokens)
    {
        _properties = (
            from property in queryType.GetProperties()
            where !routeTokens.Contains(property.Name)
            where property.GetCustomAttribute<JsonIgnoreAttribute>() is null
            select new QueryEncoderProperty(property, queryType)).ToArray();
    }

    internal string Encode(IHttpQuery query)
    {
        var writer = new QueryWriter();

        foreach(var property in _properties)
        {
            property.Write("", query, writer);
        }

        return writer.ToString();
    }
}
