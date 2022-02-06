using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Totem.Workflows;

public class WorkflowPipelineBuilder : IWorkflowPipelineBuilder
{
    readonly IServiceProvider _services;
    readonly ILoggerFactory _loggerFactory;
    readonly IWorkflowContextFactory _contextFactory;
    readonly List<IWorkflowMiddleware> _steps = new();

    public WorkflowPipelineBuilder(
        IServiceProvider services,
        ILoggerFactory loggerFactory,
        IWorkflowContextFactory contextFactory)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id PipelineId { get; set; } = Id.NewId();

    public IWorkflowPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IWorkflowMiddleware
    {
        if(middleware is not null)
        {
            _steps.Add(middleware);
        }
        else
        {
            _steps.Add(new WorkflowMiddleware<TMiddleware>(_services));
        }

        return this;
    }

    public IWorkflowPipeline Build() =>
        new WorkflowPipeline(PipelineId, _loggerFactory.CreateLogger<WorkflowPipeline>(), _steps, _contextFactory);
}
