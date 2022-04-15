using Totem.Http.Queries;

namespace Totem.Hosting;

public static class HttpQueryClientPipelineExtensions
{
    public static IHttpQueryClientPipelineBuilder Use(this IHttpQueryClientPipelineBuilder builder, Func<IHttpQueryClientContext<IHttpQuery>, Func<Task>, CancellationToken, Task> middleware)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Use(new HttpQueryClientMiddleware(middleware));
    }

    public static IHttpQueryClientPipelineBuilder Use(this IHttpQueryClientPipelineBuilder builder, Func<IHttpQueryClientContext<IHttpQuery>, Func<Task>, Task> middleware)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(middleware is null)
            throw new ArgumentNullException(nameof(middleware));

        return builder.Use((context, next, _) => middleware(context, next));
    }

    public static IHttpQueryClientPipelineBuilder UseRequest(this IHttpQueryClientPipelineBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Use<HttpQueryClientRequestMiddleware>();
    }
}
