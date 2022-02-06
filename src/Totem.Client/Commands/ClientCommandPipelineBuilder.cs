using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Totem.Commands;

public class ClientCommandPipelineBuilder : IClientCommandPipelineBuilder
{
    readonly IServiceProvider _services;
    readonly ILoggerFactory _loggerFactory;
    readonly IClientCommandContextFactory _contextFactory;
    readonly List<IClientCommandMiddleware> _steps = new();

    public ClientCommandPipelineBuilder(
        IServiceProvider services,
        ILoggerFactory loggerFactory,
        IClientCommandContextFactory contextFactory)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id PipelineId { get; set; } = Id.NewId();

    public IClientCommandPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IClientCommandMiddleware
    {
        if(middleware is not null)
        {
            _steps.Add(middleware);
        }
        else
        {
            _steps.Add(new ClientCommandMiddleware<TMiddleware>(_services));
        }

        return this;
    }

    public IClientCommandPipeline Build() =>
        new ClientCommandPipeline(PipelineId, _loggerFactory.CreateLogger<ClientCommandPipeline>(), _steps, _contextFactory);
}
