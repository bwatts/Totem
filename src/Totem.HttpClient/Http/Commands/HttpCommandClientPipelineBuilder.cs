namespace Totem.Http.Commands;

public class HttpCommandClientPipelineBuilder : IHttpCommandClientPipelineBuilder
{
    readonly IServiceProvider _services;
    readonly ILoggerFactory _loggerFactory;
    readonly IHttpCommandClientContextFactory _contextFactory;
    readonly List<IHttpCommandClientMiddleware> _steps = new();

    public HttpCommandClientPipelineBuilder(
        IServiceProvider services,
        ILoggerFactory loggerFactory,
        IHttpCommandClientContextFactory contextFactory)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id PipelineId { get; set; } = Id.NewId();

    public IHttpCommandClientPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IHttpCommandClientMiddleware
    {
        if(middleware is not null)
        {
            _steps.Add(middleware);
        }
        else
        {
            _steps.Add(new HttpCommandClientMiddleware<TMiddleware>(_services));
        }

        return this;
    }

    public IHttpCommandClientPipeline Build() =>
        new HttpCommandClientPipeline(PipelineId, _loggerFactory.CreateLogger<HttpCommandClientPipeline>(), _steps, _contextFactory);
}
