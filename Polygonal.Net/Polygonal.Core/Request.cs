using System.Text;

namespace Polygonal.Core
{
    public class Request
    {
        private const string TrueString = "true", FalseString = "false";

        private StringBuilder builder = new StringBuilder(512); // avoid re-allocation by giving us plenty of size up-front
        private bool addedGetParameter = false;

        public Request() { }

        public Request(string baseUrl)
        {
            this.builder.Append(baseUrl);
        }

        public Request AddGetParameter(string key, string value)
        {
            if (!this.addedGetParameter)
            {
                this.builder.Append("?");
                this.addedGetParameter = true;
            }
            else
            {
                this.builder.Append("&");
            }

            this.builder.Append(key).Append("=").Append(value);

            return this;
        }

        public Request AddGetParameter(string key, int value) => AddGetParameter(key, ToString(value));

        public Request AddGetParameter(string key, bool value) => AddGetParameter(key, ToString(value));

        public Request AddRestParameter(string parameter)
        {
            if (this.addedGetParameter)
            {
                throw new InvalidOperationException($"Attempted to add REST parameter \"${parameter}\" after GET parameters have been added.");
            }

            this.builder.Append("/").Append(parameter);

            return this;
        }

        public Request AddRestParameter(DateTime date) => AddRestParameter(ToString(date));

        public Request AddRestParameter(int value) => AddRestParameter(ToString(value));

        internal string GetRequestUrl()
        {
            return this.builder.ToString();
        }

        private string ToString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        private string ToString(int value)
        {
            return Convert.ToString(value);
        }

        private string ToString(bool value)
        {
            return value ? TrueString : FalseString;
        }
    }
}
