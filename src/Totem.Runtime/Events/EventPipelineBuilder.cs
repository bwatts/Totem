namespace Totem.Events;

public class EventPipelineBuilder : IEventPipelineBuilder
{
    readonly IServiceProvider _services;
    readonly ILoggerFactory _loggerFactory;
    readonly IEventContextFactory _contextFactory;
    readonly List<IEventMiddleware> _steps = new();

    public EventPipelineBuilder(
        IServiceProvider services,
        ILoggerFactory loggerFactory,
        IEventContextFactory contextFactory)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id PipelineId { get; set; } = Id.NewId();

    public IEventPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IEventMiddleware
    {
        if(middleware is not null)
        {
            _steps.Add(middleware);
        }
        else
        {
            _steps.Add(new EventMiddleware<TMiddleware>(_services));
        }

        return this;
    }

    public IEventPipeline Build() =>
        new EventPipeline(PipelineId, _loggerFactory.CreateLogger<EventPipeline>(), _steps, _contextFactory);
}
