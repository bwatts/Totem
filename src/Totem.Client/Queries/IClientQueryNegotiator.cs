using System.Net.Http;

namespace Totem.Queries
{
    public interface IClientQueryNegotiator
    {
        HttpRequestMessage Negotiate(IHttpQuery query);
        void NegotiateResult(IClientQueryContext<IHttpQuery> context);
    }
}