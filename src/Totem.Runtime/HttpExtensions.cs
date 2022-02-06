using System.Net;
using Totem.Http;

namespace Totem;

public static class HttpExtensions
{
    public static void RespondOk<TCommand>(this IHttpCommandContext<TCommand> context, object? content = null)
        where TCommand : IHttpCommand
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        context.ResponseCode = HttpStatusCode.OK;
        context.ResponseContent = content;
    }

    public static void RespondOkStreaming<TCommand>(
        this IHttpCommandContext<TCommand> context,
        Stream stream,
        string contentType = HttpStreamContent.DefaultContentType,
        string contentName = HttpStreamContent.DefaultContentName)
        where TCommand : IHttpCommand
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        context.ResponseCode = HttpStatusCode.OK;
        context.ResponseContent = new HttpStreamContent(stream, contentType, contentName);
    }

    public static void RespondCreated<TCommand>(this IHttpCommandContext<TCommand> context, string location, object? content = null)
        where TCommand : IHttpCommand
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        context.ResponseCode = HttpStatusCode.Created;
        context.ResponseHeaders["Location"] = location ?? throw new ArgumentNullException(nameof(location));
        context.ResponseContent = content;
    }

    public static void RespondCreated<TCommand>(this IHttpCommandContext<TCommand> context, Uri location, object? content = null)
        where TCommand : IHttpCommand
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        if(location is null)
            throw new ArgumentNullException(nameof(location));

        context.RespondCreated(location.ToString(), content);
    }

    public static void RespondAccepted<TCommand>(this IHttpCommandContext<TCommand> context, object? content = null)
        where TCommand : IHttpCommand
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        context.ResponseCode = HttpStatusCode.Accepted;
        context.ResponseContent = content;
    }

    public static void RespondNoContent<TCommand>(this IHttpCommandContext<TCommand> context)
        where TCommand : IHttpCommand
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        context.ResponseCode = HttpStatusCode.NoContent;
        context.ResponseContent = null;
    }

    public static void RespondOk<TQuery>(this IHttpQueryContext<TQuery> context, object? result)
        where TQuery : IHttpQuery
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        context.ResponseCode = HttpStatusCode.OK;
        context.Result = result;
    }

    public static void RespondOk<TQuery, TResult>(this IHttpQueryContext<TQuery> context, TResult result)
        where TQuery : IHttpQuery<TResult>
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        context.ResponseCode = HttpStatusCode.OK;
        context.Result = result;
    }

    public static void RespondOkStreaming<TQuery>(
        this IHttpQueryContext<TQuery> context,
        Stream stream,
        string contentType = HttpStreamContent.DefaultContentType,
        string contentName = HttpStreamContent.DefaultContentName)
        where TQuery : IHttpQuery
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        context.ResponseCode = HttpStatusCode.OK;
        context.Result = new HttpStreamContent(stream, contentType, contentName);
    }

    public static void RespondNoContent<TQuery, TResult>(this IHttpQueryContext<TQuery> context)
        where TQuery : IHttpQuery<TResult>
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        context.ResponseCode = HttpStatusCode.NoContent;
        context.Result = null;
    }
}
