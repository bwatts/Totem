using System;
using Serilog;
using Serilog.Configuration;
using Totem.Core;
using Totem.Events;

namespace Totem.Hosting;

public static class SerilogExtensions
{
    public static LoggerConfiguration ShortIds(this LoggerDestructuringConfiguration destructure)
    {
        if(destructure is null)
            throw new ArgumentNullException(nameof(destructure));

        return destructure.ByTransforming<Id>(id => id.ToShortString());
    }

    public static LoggerConfiguration WithShortTypes(this LoggerEnrichmentConfiguration enrich, Func<Type, bool> useShortType)
    {
        if(enrich is null)
            throw new ArgumentNullException(nameof(enrich));

        return enrich.With(new ShortTypeEnricher(useShortType));
    }

    public static LoggerConfiguration WithShortTotemTypes(this LoggerEnrichmentConfiguration enrich) =>
        enrich.WithShortTypes(type => typeof(IMessage).IsAssignableFrom(type) || typeof(ITimeline).IsAssignableFrom(type));
}
