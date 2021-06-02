using System;
using Totem.Core;

namespace Totem.Http
{
    public abstract class HttpMessageInfo : MessageInfo
    {
        protected HttpMessageInfo(Type messageType, HttpRequestInfo request) : base(messageType) =>
            Request = request ?? throw new ArgumentNullException(nameof(request));

        public HttpRequestInfo Request { get; }

        public override string ToString() => $"{Request} => {MessageType}";
    }
}