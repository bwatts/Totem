using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Totem.Queues
{
    public class QueuePipelineBuilder : IQueuePipelineBuilder
    {
        readonly IServiceProvider _services;
        readonly ILoggerFactory _loggerFactory;
        readonly IQueueContextFactory _contextFactory;
        readonly List<IQueueMiddleware> _steps = new();

        public QueuePipelineBuilder(
            IServiceProvider services,
            ILoggerFactory loggerFactory,
            IQueueContextFactory contextFactory)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public Id PipelineId { get; set; } = Id.NewId();

        public IQueuePipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IQueueMiddleware
        {
            if(middleware != null)
            {
                _steps.Add(middleware);
            }
            else
            {
                _steps.Add(new QueueMiddleware<TMiddleware>(_services));
            }

            return this;
        }

        public IQueuePipeline Build() =>
            new QueuePipeline(PipelineId, _loggerFactory.CreateLogger<QueuePipeline>(), _steps, _contextFactory);
    }
}