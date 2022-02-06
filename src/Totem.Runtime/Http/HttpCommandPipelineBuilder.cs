using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Totem.Http;

public class HttpCommandPipelineBuilder : IHttpCommandPipelineBuilder
{
    readonly IServiceProvider _services;
    readonly ILoggerFactory _loggerFactory;
    readonly IHttpCommandContextFactory _contextFactory;
    readonly List<IHttpCommandMiddleware> _steps = new();

    public HttpCommandPipelineBuilder(
        IServiceProvider services,
        ILoggerFactory loggerFactory,
        IHttpCommandContextFactory contextFactory)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id PipelineId { get; set; } = Id.NewId();

    public IHttpCommandPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IHttpCommandMiddleware
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

    public IHttpCommandPipeline Build() =>
        new HttpCommandPipeline(PipelineId, _loggerFactory.CreateLogger<HttpCommandPipeline>(), _steps, _contextFactory);
}
