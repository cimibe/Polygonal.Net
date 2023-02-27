using System.Threading.RateLimiting;

namespace Polygonal.Core
{
    /// <summary>
    /// Enum representing different limits from the Polygon.io API. Note that the actual limits imposed by Polygon.io are subject to
    /// change, and those changes may not get reflected here.
    /// </summary>
    public enum ApiLimit { Basic, FivePerMinute, Unlimited }

    /// <summary>
    /// Simple factory implementation that allows for the construction of PolygonClient instances with different parameters.
    /// </summary>
    public static class ClientFactory
    {
        /// <summary>
        /// Gets a new PolygonClient instance with an internally allocated HttpClient and the provided RateLimiter and API key.
        /// </summary>
        /// <param name="apiKey">API key for communicating with the service</param>
        /// <param name="limiter">RateLimiter instance to handle client-side throttling</param>
        /// <param name="diposeLimiter">Whether or not calling Dispose() on the client will dispose of the RateLimiter instance</param>
        /// <returns>A new PolygonClient instance</returns>
        public static PolygonClient GetClient(string apiKey, RateLimiter limiter, bool diposeLimiter)
        {
            return new PolygonClient(apiKey, new HttpClient(), true, limiter, diposeLimiter);
        }

        /// <summary>
        /// Gets a new PolygonClient instance with the provided API key, an internally allocated HttpClient and RateLimiter based on the
        /// specified API limits.
        /// </summary>
        /// <param name="apiKey">API key for communicating with the service</param>
        /// <param name="limit">Limits for the provided API key</param>
        /// <returns>A new PolygonClient instance</returns>
        public static PolygonClient GetClient(string apiKey, ApiLimit limit)
        {
            RateLimiter limiter;
            switch (limit)
            {
                case ApiLimit.Basic:
                case ApiLimit.FivePerMinute:
                    FixedWindowRateLimiterOptions options = new FixedWindowRateLimiterOptions();
                    options.PermitLimit = 5;
                    options.QueueLimit = Int32.MaxValue;
                    options.AutoReplenishment = true;
                    options.Window = TimeSpan.FromMinutes(1);

                    limiter = new FixedWindowRateLimiter(options);
                    break;
                case ApiLimit.Unlimited:
                    limiter = GetNoOpRateLimiter();
                    break;
                default:
                    limiter = GetNoOpRateLimiter();
                    break;
            }

            return GetClient(apiKey, limiter, true);
        }

        /// <summary>
        /// Gets a new PolygonClient instance with the provided API key, an internally allocated HttpClient and no RateLimiter.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns>A new PolygonClient instance</returns>
        public static PolygonClient GetClient(string apiKey)
        {
            return GetClient(apiKey, ApiLimit.Unlimited);
        }

        private static RateLimiter GetNoOpRateLimiter()
        {
            // This would be a bit simpler if the NoopLimiter were exposed publicly, but as it is the only way to get
            // to it is to go through a RateLimitPartition. The actual parameters here don't matter for a NoopLimiter
            // , so just for simplicity we'll use null.
            return RateLimitPartition.GetNoLimiter<PolygonClient?>(null).Factory.Invoke(null);
        }
    }
}
