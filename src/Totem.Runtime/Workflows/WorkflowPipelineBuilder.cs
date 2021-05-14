using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Totem.Routes;

namespace Totem.Workflows
{
    public class WorkflowPipelineBuilder : IWorkflowPipelineBuilder
    {
        readonly IServiceProvider _services;
        readonly ILoggerFactory _loggerFactory;
        readonly IRouteContextFactory _contextFactory;
        readonly List<IRouteMiddleware> _steps = new();

        public WorkflowPipelineBuilder(
            IServiceProvider services,
            ILoggerFactory loggerFactory,
            IRouteContextFactory contextFactory)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public Id PipelineId { get; set; } = Id.NewId();

        public IWorkflowPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IRouteMiddleware
        {
            if(middleware != null)
            {
                _steps.Add(middleware);
            }
            else
            {
                _steps.Add(new RouteMiddleware<TMiddleware>(_services));
            }

            return this;
        }

        public IWorkflowPipeline Build() =>
            new WorkflowPipeline(PipelineId, _loggerFactory.CreateLogger<WorkflowPipeline>(), _steps, _contextFactory);
    }
}