namespace Totem.Topics;

public class TopicPipelineBuilder : ITopicPipelineBuilder
{
    readonly IServiceProvider _services;
    readonly ILoggerFactory _loggerFactory;
    readonly ITopicContextFactory _contextFactory;
    readonly List<ITopicMiddleware> _steps = new();

    public TopicPipelineBuilder(
        IServiceProvider services,
        ILoggerFactory loggerFactory,
        ITopicContextFactory contextFactory)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    public Id PipelineId { get; set; } = Id.NewId();

    public ITopicPipelineBuilder Use<TMiddleware>(TMiddleware? middleware = default) where TMiddleware : ITopicMiddleware
    {
        if(middleware is not null)
        {
            _steps.Add(middleware);
        }
        else
        {
            _steps.Add(new ReportMiddleware<TMiddleware>(_services));
        }

        return this;
    }

    public ITopicPipeline Build() =>
        new TopicPipeline(PipelineId, _loggerFactory.CreateLogger<TopicPipeline>(), _steps, _contextFactory);
}
