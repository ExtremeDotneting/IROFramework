using System.Collections.Generic;
using Bridge.Html5;
using Newtonsoft.Json;

namespace System.Net.Http
{
    public class HttpResponseMessage
    {
        [JsonIgnore]
        public XMLHttpRequest InternalRequest { get; set; }

        public int StatusCode { get; set; }

        public HttpContent Content { get; set; }

        public IDictionary<string, string> Headers { get; set; } 

    }
}