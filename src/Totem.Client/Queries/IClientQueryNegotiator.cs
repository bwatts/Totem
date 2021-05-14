using System;
using System.Net.Http;

namespace Totem.Queries
{
    public interface IClientQueryNegotiator
    {
        HttpRequestMessage Negotiate(IQuery query);
        object? NegotiateResult(Type resultType, string contentType, string content);
    }
}