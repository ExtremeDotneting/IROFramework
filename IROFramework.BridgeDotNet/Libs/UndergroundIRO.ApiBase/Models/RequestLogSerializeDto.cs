using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace UndergroundIRO.ApiBase.Models
{
    class RequestLogSerializeDto
    {
        public RequestLogSerializeDto() { }

        public RequestLogSerializeDto(
            string url,
            HttpMethod method,
            IDictionary<string, string> headerParams,
            IDictionary<string, string> formParams,
            Encoding textContentEncoding,
            string mediaType,
            string stringsBody,
            HttpContent httpContent
            )
        {
            Url = url;
            Method = method;
            HeaderParams = headerParams;
            if (stringsBody != null)
            {
                TextContentEncoding = textContentEncoding;
                StringsBody = stringsBody;
                MediaType = mediaType;
            }
            else if (formParams != null)
            {
                FormParams = formParams;
            }
            else if (httpContent != null)
            {
                UsedCustomHttpContent = true;
            }
        }

        public string Url { get; set; }
        public HttpMethod Method { get; set; }

        public IDictionary<string, string> HeaderParams { get; set; }

        public Encoding TextContentEncoding { get; set; }

        public string StringsBody { get; set; }

        public string MediaType { get; set; }

        public IDictionary<string, string> FormParams { get; set; }
        
        public bool UsedCustomHttpContent { get; set; }

        public override string ToString()
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
            return JsonConvert.SerializeObject(
                this,
                Formatting.Indented,
                settings
                );
        }
    }
}
