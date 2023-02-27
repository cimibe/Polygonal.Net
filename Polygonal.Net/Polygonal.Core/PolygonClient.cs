using System.Net.Http.Json;
using System.Threading.RateLimiting;

namespace Polygonal.Core
{
    /// <summary>
    /// This is the base Polygon.io client, used for sending raw requests to the service and deserializing the response.
    /// </summary>
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

        /// <summary>
        /// Sends an asynchronous request to the service with a URL derived from the specific request instance.
        /// </summary>
        /// <typeparam name="T">Type of the expected return value (for deserialization)</typeparam>
        /// <param name="request">Request to be made</param>
        /// <param name="cancel">Token to cancel the request</param>
        /// <returns>Task result encapsulating the response value</returns>
        public async Task<T?> SendRequestAsync<T>(Request request, CancellationToken cancel = default)
        {
            using RateLimitLease lease = await _limiter.AcquireAsync(1, cancel);

            request.AddGetParameter(ApiKeyParameterName, _apiKey);
            return await _client.GetFromJsonAsync<T>(request.GetRequestUrl(), cancel);
        }

        /// <summary>
        /// Disposes of the instance and any internal resources that this instance is responsible for.
        /// </summary>
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