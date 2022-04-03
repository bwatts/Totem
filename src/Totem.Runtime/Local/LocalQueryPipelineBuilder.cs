namespace Totem.Local;

public class LocalQueryPipelineBuilder : ILocalQueryPipelineBuilder
{
    readonly IServiceProvider _services;
    readonly ILoggerFactory _loggerFactory;
    readonly ILocalQueryContextFactory _contextFactory;
    readonly List<ILocalQueryMiddleware> _steps = new();

    public LocalQueryPipelineBuilder(
        IServiceProvider services,
        ILoggerFactory loggerFactory,
        ILocalQueryContextFactory contextFactory)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id PipelineId { get; set; } = Id.NewId();

    public ILocalQueryPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : ILocalQueryMiddleware
    {
        if(middleware is not null)
        {
            _steps.Add(middleware);
        }
        else
        {
            _steps.Add(new LocalQueryMiddleware<TMiddleware>(_services));
        }

        return this;
    }

    public ILocalQueryPipeline Build() =>
        new LocalQueryPipeline(PipelineId, _loggerFactory.CreateLogger<LocalQueryPipeline>(), _steps, _contextFactory);
}
