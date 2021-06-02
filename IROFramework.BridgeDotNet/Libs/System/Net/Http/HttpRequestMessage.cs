using System.Collections.Generic;

namespace System.Net.Http
{
    public class HttpRequestMessage
    {
        /// <summary>Gets or sets the contents of the HTTP message.</summary>
        /// <returns>The content of a message</returns>
        public HttpContent Content { get; set; }

        /// <summary>Gets the collection of HTTP request headers.</summary>
        /// <returns>The collection of HTTP request headers.</returns>
        public IDictionary<string, string> Headers { get; } = new PlainDictionary<string, string>();

        public HttpMethod Method { get; set; }

        public Uri RequestUri { get; set; }

        public HttpRequestMessage(HttpMethod method, string url)
        {
            Method = method;
            RequestUri = new Uri(url);
        }

        public HttpRequestMessage(HttpMethod method, Uri requestUri)
        {
            Method = method;
            RequestUri = requestUri;
        }
    }
}