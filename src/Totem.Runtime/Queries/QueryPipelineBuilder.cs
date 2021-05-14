using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Totem.Queries
{
    public class QueryPipelineBuilder : IQueryPipelineBuilder
    {
        readonly IServiceProvider _services;
        readonly ILoggerFactory _loggerFactory;
        readonly IQueryContextFactory _contextFactory;
        readonly List<IQueryMiddleware> _steps = new();

        public QueryPipelineBuilder(
            IServiceProvider services,
            ILoggerFactory loggerFactory,
            IQueryContextFactory contextFactory)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public Id PipelineId { get; set; } = Id.NewId();

        public IQueryPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IQueryMiddleware
        {
            if(middleware != null)
            {
                _steps.Add(middleware);
            }
            else
            {
                _steps.Add(new QueryMiddleware<TMiddleware>(_services));
            }

            return this;
        }

        public IQueryPipeline Build() =>
            new QueryPipeline(PipelineId, _loggerFactory.CreateLogger<QueryPipeline>(), _steps, _contextFactory);
    }
}