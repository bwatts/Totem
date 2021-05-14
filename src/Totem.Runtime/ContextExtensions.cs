using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Totem.Events;

namespace Totem
{
    public static class ContextExtensions
    {
        public static void RespondOk<TCommand>(this ICommandContext<TCommand> context, object? content = null)
            where TCommand : ICommand
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            context.ResponseCode = HttpStatusCode.OK;
            context.ResponseContent = content;
        }

        public static void RespondOkStreaming<TCommand>(
            this ICommandContext<TCommand> context,
            Stream stream,
            string contentType = StreamContent.DefaultContentType,
            string contentName = StreamContent.DefaultContentName)
            where TCommand : ICommand
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            context.ResponseCode = HttpStatusCode.OK;
            context.ResponseContent = new StreamContent(stream, contentType, contentName);
        }

        public static void RespondCreated<TCommand>(this ICommandContext<TCommand> context, string location, object? content = null)
            where TCommand : ICommand
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            context.ResponseCode = HttpStatusCode.Created;
            context.ResponseHeaders["Location"] = location ?? throw new ArgumentNullException(nameof(location));
            context.ResponseContent = content;
        }

        public static void RespondCreated<TCommand>(this ICommandContext<TCommand> context, Uri location, object? content = null)
            where TCommand : ICommand
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            if(location == null)
                throw new ArgumentNullException(nameof(location));

            context.RespondCreated(location.ToString(), content);
        }

        public static void RespondAccepted<TCommand>(this ICommandContext<TCommand> context, object? content = null)
            where TCommand : ICommand
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            context.ResponseCode = HttpStatusCode.Accepted;
            context.ResponseContent = content;
        }

        public static void RespondNoContent<TCommand>(this ICommandContext<TCommand> context)
            where TCommand : ICommand
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            context.ResponseCode = HttpStatusCode.NoContent;
            context.ResponseContent = null;
        }

        public static void RespondOk<TQuery, TResult>(this IQueryContext<TQuery> context, TResult result)
            where TQuery : IQuery<TResult>
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            context.ResponseCode = HttpStatusCode.OK;
            context.ResponseContent = result;
        }

        public static void RespondOkStreaming<TQuery>(
            this IQueryContext<TQuery> context,
            Stream stream,
            string contentType = StreamContent.DefaultContentType,
            string contentName = StreamContent.DefaultContentName)
            where TQuery : IQuery
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            context.ResponseCode = HttpStatusCode.OK;
            context.ResponseContent = new StreamContent(stream, contentType, contentName);
        }

        public static void RespondNoContent<TQuery, TResult>(this IQueryContext<TQuery> context)
            where TQuery : IQuery<TResult>
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            context.ResponseCode = HttpStatusCode.NoContent;
            context.ResponseContent = null;
        }
    }
}