namespace Totem.Http.Queries;

public interface IHttpQueryNegotiator
{
    HttpRequestMessage Negotiate(IHttpQuery query);
    void NegotiateResult(IHttpQueryClientContext<IHttpQuery> context);
}
