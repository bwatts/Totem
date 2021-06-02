using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Totem.Queues
{
    public class QueueCommandPipelineBuilder : IQueueCommandPipelineBuilder
    {
        readonly IServiceProvider _services;
        readonly ILoggerFactory _loggerFactory;
        readonly IQueueCommandContextFactory _contextFactory;
        readonly List<IQueueCommandMiddleware> _steps = new();

        public QueueCommandPipelineBuilder(
            IServiceProvider services,
            ILoggerFactory loggerFactory,
            IQueueCommandContextFactory contextFactory)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public Id PipelineId { get; set; } = Id.NewId();

        public IQueueCommandPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IQueueCommandMiddleware
        {
            if(middleware != null)
            {
                _steps.Add(middleware);
            }
            else
            {
                _steps.Add(new QueueCommandMiddleware<TMiddleware>(_services));
            }

            return this;
        }

        public IQueueCommandPipeline Build() =>
            new QueueCommandPipeline(PipelineId, _loggerFactory.CreateLogger<QueueCommandPipeline>(), _steps, _contextFactory);
    }
}