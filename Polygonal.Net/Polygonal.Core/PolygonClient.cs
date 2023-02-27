using System.Net.Http.Json;
using System.Threading.RateLimiting;

namespace Polygonal.Core
{
    public class PolygonClient : IDisposable
    {
        private const string ApiKeyParameterName = "apiKey";

        private readonly string _apiKey;

        private readonly bool _disposeClient;
        private readonly HttpClient _client;

        private readonly bool _disposeLimiter;
        private readonly RateLimiter _limiter;

        internal PolygonClient(string apiKey, HttpClient client, bool disposeClient, RateLimiter limiter, bool disposeLimiter)
        {
            _apiKey = apiKey;

            _client = client;
            _disposeClient = disposeClient;

            _limiter = limiter;
            _disposeLimiter = disposeLimiter;
        }

        public async Task<T?> SendRequestAsync<T>(Request request, CancellationToken cancel = default)
        {
            using RateLimitLease lease = await _limiter.AcquireAsync(1, cancel);

            request.AddGetParameter(ApiKeyParameterName, _apiKey);
            return await _client.GetFromJsonAsync<T>(request.GetRequestUrl(), cancel);
        }

        public void Dispose()
        {
            if (_disposeLimiter)
            {
                _limiter.Dispose();
            }

            if (_disposeClient)
            {
                _client.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}