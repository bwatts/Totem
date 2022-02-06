using System;
using System.Collections.Specialized;
using System.Net;
using Totem.Core;
using Totem.Map;

namespace Totem.Http;

public class HttpQueryContext<TQuery> : QueryContext<TQuery>, IHttpQueryContext<TQuery>
    where TQuery : IHttpQuery
{
    internal HttpQueryContext(Id pipelineId, IHttpQueryEnvelope envelope, QueryType queryType) : base(pipelineId, envelope, queryType)
    { }

    public new IHttpQueryEnvelope Envelope => (IHttpQueryEnvelope) base.Envelope;
    public new HttpQueryInfo Info => (HttpQueryInfo) base.Info;
    public override Type InterfaceType => typeof(IHttpQueryContext<TQuery>);
    public HttpStatusCode ResponseCode { get; set; } = HttpStatusCode.OK;
    public NameValueCollection ResponseHeaders { get; } = new(StringComparer.OrdinalIgnoreCase);
    public string? ResponseContentType { get; set; }
}
