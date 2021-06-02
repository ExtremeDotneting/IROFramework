using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace UndergroundIRO.ApiBase.Models
{
    public class ApiResponse<T>
    {
        public HttpResponseMessage HttpResponseMessage { get; set; }

        /// <summary>
        /// Gets or sets the status code (HTTP status code)
        /// </summary>
        /// <value>The status code.</value>
        public int StatusCode => HttpResponseMessage.StatusCode;

        /// <summary>
        /// Gets or sets the HTTP headers
        /// </summary>
        /// <value>HTTP headers</value>
        public IDictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Gets or sets the data (parsed HTTP body)
        /// </summary>
        /// <value>The data.</value>
        public T Data { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponse&lt;T&gt;" /> class.
        /// </summary>
        /// <param name="statusCode">HTTP status code.</param>
        /// <param name="headers">HTTP headers.</param>
        /// <param name="data">Data (parsed HTTP body)</param>
        public ApiResponse(IDictionary<string, string> headers, T data, HttpResponseMessage httpResponseMessage)
        {
            HttpResponseMessage = httpResponseMessage;
            this.Headers = headers;
            this.Data = data;
        }

        public override string ToString()
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
            return JsonConvert.SerializeObject(this, Formatting.Indented, settings);
        }
    }
}
