using System.Threading.RateLimiting;

namespace Polygonal.Core
{
    public enum ApiLimit { Basic, Unlimited }

    public static class ClientFactory
    {
        public static PolygonClient GetClient(string apiKey, RateLimiter limiter, bool diposeLimiter)
        {
            return new PolygonClient(apiKey, new HttpClient(), true, limiter, diposeLimiter);
        }

        public static PolygonClient GetClient(string apiKey, ApiLimit limit)
        {
            RateLimiter limiter;
            switch (limit)
            {
                case ApiLimit.Basic:
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
