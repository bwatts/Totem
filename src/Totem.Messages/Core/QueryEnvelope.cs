using System;
using System.Security.Claims;

namespace Totem.Core
{
    public class QueryEnvelope : MessageEnvelope, IQueryEnvelope
    {
        public QueryEnvelope(IQuery query, Id messageId, Id correlationId, ClaimsPrincipal principal)
            : base(query, messageId, correlationId, principal)
        {
            Message = query;
            ResultType = QueryInfo.From(query).ResultType;
        }

        public new IQuery Message { get; }
        public Type ResultType { get; }
    }
}