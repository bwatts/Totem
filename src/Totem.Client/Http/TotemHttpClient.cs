using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Http
{
    public class TotemHttpClient : ITotemHttpClient
    {
        readonly HttpClient _httpClient;

        public TotemHttpClient(HttpClient httpClient) =>
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        public async Task SendAsync(ITotemHttpMessage message, CancellationToken cancellationToken)
        {
            if(message == null)
                throw new ArgumentNullException(nameof(message));

            await message.SendAsync(_httpClient, cancellationToken);
        }
    }
}