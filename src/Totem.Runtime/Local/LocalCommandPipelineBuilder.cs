using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Totem.Local;

public class LocalCommandPipelineBuilder : ILocalCommandPipelineBuilder
{
    readonly IServiceProvider _services;
    readonly ILoggerFactory _loggerFactory;
    readonly ILocalCommandContextFactory _contextFactory;
    readonly List<ILocalCommandMiddleware> _steps = new();

    public LocalCommandPipelineBuilder(
        IServiceProvider services,
        ILoggerFactory loggerFactory,
        ILocalCommandContextFactory contextFactory)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id PipelineId { get; set; } = Id.NewId();

    public ILocalCommandPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : ILocalCommandMiddleware
    {
        if(middleware is not null)
        {
            _steps.Add(middleware);
        }
        else
        {
            _steps.Add(new HttpCommandMiddleware<TMiddleware>(_services));
        }

        return this;
    }

    public ILocalCommandPipeline Build() =>
        new LocalCommandPipeline(PipelineId, _loggerFactory.CreateLogger<LocalCommandPipeline>(), _steps, _contextFactory);
}
