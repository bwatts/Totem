namespace Totem.Http.Queries;

public class HttpQueryClientPipelineBuilder : IHttpQueryClientPipelineBuilder
{
    readonly IServiceProvider _services;
    readonly ILoggerFactory _loggerFactory;
    readonly IHttpQueryClientContextFactory _contextFactory;
    readonly List<IHttpQueryClientMiddleware> _steps = new();

    public HttpQueryClientPipelineBuilder(
        IServiceProvider services,
        ILoggerFactory loggerFactory,
        IHttpQueryClientContextFactory contextFactory)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id PipelineId { get; set; } = Id.NewId();

    public IHttpQueryClientPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IHttpQueryClientMiddleware
    {
        if(middleware is not null)
        {
            _steps.Add(middleware);
        }
        else
        {
            _steps.Add(new HttpQueryClientMiddleware<TMiddleware>(_services));
        }

        return this;
    }

    public IHttpQueryClientPipeline Build() =>
        new HttpQueryClientPipeline(PipelineId, _loggerFactory.CreateLogger<HttpQueryClientPipeline>(), _steps, _contextFactory);
}
