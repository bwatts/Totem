using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Totem.Queries
{
    public class ClientQueryPipelineBuilder : IClientQueryPipelineBuilder
    {
        readonly IServiceProvider _services;
        readonly ILoggerFactory _loggerFactory;
        readonly IClientQueryContextFactory _contextFactory;
        readonly List<IClientQueryMiddleware> _steps = new();

        public ClientQueryPipelineBuilder(
            IServiceProvider services,
            ILoggerFactory loggerFactory,
            IClientQueryContextFactory contextFactory)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public Id PipelineId { get; set; } = Id.NewId();

        public IClientQueryPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IClientQueryMiddleware
        {
            if(middleware != null)
            {
                _steps.Add(middleware);
            }
            else
            {
                _steps.Add(new ClientQueryMiddleware<TMiddleware>(_services));
            }

            return this;
        }

        public IClientQueryPipeline Build() =>
            new ClientQueryPipeline(PipelineId, _loggerFactory.CreateLogger<ClientQueryPipeline>(), _steps, _contextFactory);
    }
}