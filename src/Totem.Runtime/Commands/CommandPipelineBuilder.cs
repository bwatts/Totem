using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Totem.Commands
{
    public class CommandPipelineBuilder : ICommandPipelineBuilder
    {
        readonly IServiceProvider _services;
        readonly ILoggerFactory _loggerFactory;
        readonly ICommandContextFactory _contextFactory;
        readonly List<ICommandMiddleware> _steps = new();

        public CommandPipelineBuilder(
            IServiceProvider services,
            ILoggerFactory loggerFactory,
            ICommandContextFactory contextFactory)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public Id PipelineId { get; set; } = Id.NewId();

        public ICommandPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : ICommandMiddleware
        {
            if(middleware != null)
            {
                _steps.Add(middleware);
            }
            else
            {
                _steps.Add(new CommandMiddleware<TMiddleware>(_services));
            }

            return this;
        }

        public ICommandPipeline Build() =>
            new CommandPipeline(PipelineId, _loggerFactory.CreateLogger<CommandPipeline>(), _steps, _contextFactory);
    }
}