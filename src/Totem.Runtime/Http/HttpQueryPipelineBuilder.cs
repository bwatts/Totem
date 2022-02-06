using Microsoft.Extensions.Logging;

namespace Totem.Http;

public class HttpQueryPipelineBuilder : IHttpQueryPipelineBuilder
{
    readonly IServiceProvider _services;
    readonly ILoggerFactory _loggerFactory;
    readonly IHttpQueryContextFactory _contextFactory;
    readonly List<IHttpQueryMiddleware> _steps = new();

    public HttpQueryPipelineBuilder(
        IServiceProvider services,
        ILoggerFactory loggerFactory,
        IHttpQueryContextFactory contextFactory)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id PipelineId { get; set; } = Id.NewId();

    public IHttpQueryPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : IHttpQueryMiddleware
    {
        if(middleware is not null)
        {
            _steps.Add(middleware);
        }
        else
        {
            _steps.Add(new HttpQueryMiddleware<TMiddleware>(_services));
        }

        return this;
    }

    public IHttpQueryPipeline Build() =>
        new HttpQueryPipeline(PipelineId, _loggerFactory.CreateLogger<HttpQueryPipeline>(), _steps, _contextFactory);
}
