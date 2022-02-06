using Serilog.Core;
using Serilog.Events;

namespace Totem.Hosting;

public class ShortTypeEnricher : ILogEventEnricher
{
    readonly Func<Type, bool> _useShortType;

    public ShortTypeEnricher(Func<Type, bool> useShortType) =>
        _useShortType = useShortType ?? throw new ArgumentNullException(nameof(useShortType));

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if(logEvent is null)
            throw new ArgumentNullException(nameof(logEvent));

        if(propertyFactory is null)
            throw new ArgumentNullException(nameof(propertyFactory));

        List<LogEventProperty>? updates = null;

        foreach(var property in logEvent.Properties)
        {
            if(TryGetShortType(property, out var shortTypeProperty))
            {
                updates ??= new List<LogEventProperty>();
                updates.Add(shortTypeProperty);
            }
        }

        if(updates is not null)
        {
            foreach(var update in updates)
            {
                logEvent.AddOrUpdateProperty(update);
            }
        }
    }

    bool TryGetShortType(KeyValuePair<string, LogEventPropertyValue> property, out LogEventProperty shortTypeProperty)
    {
        if(property.Value is ScalarValue { Value: Type type } && _useShortType(type))
        {
            shortTypeProperty = new LogEventProperty(property.Key, new ScalarValue(type.Name));
            return true;
        }

        shortTypeProperty = null!;
        return false;
    }
}
