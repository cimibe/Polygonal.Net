namespace Polygonal.Core
{
    public class PolygonClient : IDisposable
    {
        private bool disposeHttpClient;
        private HttpClient httpClient;

        public PolygonClient(HttpClient httpClient, bool disposeHttpClient)
        {
            this.httpClient = httpClient;
            this.disposeHttpClient = disposeHttpClient;
        }

        public void Dispose()
        {
            if (this.disposeHttpClient)
            {
                this.httpClient.Dispose();
            }
        }
    }
}