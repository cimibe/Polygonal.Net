using System.Text;

namespace Polygonal.Core
{
    /// <summary>
    /// Classes is a helper for incrementally building up a request URL from a set of parameters.
    /// </summary>
    public class Request
    {
        private const string TrueString = "true", FalseString = "false";

        private StringBuilder builder = new StringBuilder(512); // avoid re-allocation by giving us plenty of size up-front
        private bool addedGetParameter = false;

        /// <summary>
        /// Creates a new Request instance with a completely empty URL.
        /// </summary>
        public Request() { }

        /// <summary>
        /// Creates a new Request instance with a given base URL as a starting point.
        /// </summary>
        /// <param name="baseUrl">Base URL of the desired request</param>
        public Request(string baseUrl)
        {
            this.builder.Append(baseUrl);
        }

        /// <summary>
        /// Adds a new GET parameter of the form "key=value" to the request URL.
        /// </summary>
        /// <param name="key">Key name</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>The current request instance</returns>
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

        /// <summary>
        /// Adds a new GET parameter of the form "key=value" to the request URL. This function is just a helper that
        /// takes in an integer value and does the string conversion internally.
        /// </summary>
        /// <param name="key">Key name</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>The current request instance</returns>
        public Request AddGetParameter(string key, int value) => AddGetParameter(key, ToString(value));

        /// <summary>
        /// Adds a new GET parameter of the form "key=value" to the request URL. This function is just a helper that
        /// takes in a bool value and does the string conversion internally.
        /// </summary>
        /// <param name="key">Key name</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>The current request instance</returns>
        public Request AddGetParameter(string key, bool value) => AddGetParameter(key, ToString(value));

        /// <summary>
        /// Adds a new REST parameter of the form "/parameter" to the request URL.
        /// </summary>
        /// <param name="parameter">Parameter value</param>
        /// <returns>The current request instance</returns>
        /// <exception cref="InvalidOperationException">Throws InvalidOperationException if attempting to add REST parameters
        /// after GET parameters.</exception>
        public Request AddRestParameter(string parameter)
        {
            if (this.addedGetParameter)
            {
                throw new InvalidOperationException($"Attempted to add REST parameter \"${parameter}\" after GET parameters have been added.");
            }

            this.builder.Append("/").Append(parameter);

            return this;
        }

        /// <summary>
        /// Adds a new REST parameter of the form "/parameter" to the request URL. This function is just a helper that
        /// takes in a DateTime value and does the string conversion internally.
        /// </summary>
        /// <param name="parameter">Parameter value</param>
        /// <returns>The current request instance</returns>
        /// <exception cref="InvalidOperationException">Throws InvalidOperationException if attempting to add REST parameters
        /// after GET parameters.</exception>
        public Request AddRestParameter(DateTime date) => AddRestParameter(ToString(date));

        /// <summary>
        /// Adds a new REST parameter of the form "/parameter" to the request URL. This function is just a helper that
        /// takes in an integer value and does the string conversion internally.
        /// </summary>
        /// <param name="parameter">Parameter value</param>
        /// <returns>The current request instance</returns>
        /// <exception cref="InvalidOperationException">Throws InvalidOperationException if attempting to add REST parameters
        /// after GET parameters.</exception>
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
