using Totem.Http.Commands;

namespace Totem.Hosting;

public static class HttpCommandClientPipelineExtensions
{
    public static IHttpCommandClientPipelineBuilder Use(this IHttpCommandClientPipelineBuilder builder, Func<IHttpCommandClientContext<IHttpCommand>, Func<Task>, CancellationToken, Task> middleware)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Use(new HttpCommandClientMiddleware(middleware));
    }

    public static IHttpCommandClientPipelineBuilder Use(this IHttpCommandClientPipelineBuilder builder, Func<IHttpCommandClientContext<IHttpCommand>, Func<Task>, Task> middleware)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(middleware is null)
            throw new ArgumentNullException(nameof(middleware));

        return builder.Use((context, next, _) => middleware(context, next));
    }

    public static IHttpCommandClientPipelineBuilder UseRequest(this IHttpCommandClientPipelineBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        return builder.Use<HttpCommandClientRequestMiddleware>();
    }
}
