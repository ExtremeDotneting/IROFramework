using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IROFramework.Core.Tools.HttpExt
{
    public class HttpClientWrapper
    {
        readonly HttpClient _client;
        readonly ILogger _logger;

        public JsonSerializerSettings SerializerSettings { get; set; } = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public HttpClientWrapper(HttpClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<JToken> Call(
            HttpMethod httpMethod,
            string endpoint,
            object requestDto = null
        )
        {
            string responseJson = null;
            string requestJson = null;
            Uri uri = null;
            int statusCode = 0;

            try
            {
                uri = new Uri($"{_client.BaseAddress.AbsoluteUri}{endpoint}");

                var httpRequestMsg = new HttpRequestMessage(httpMethod, uri);
                if (requestDto != null)
                {
                    requestJson = JsonConvert.SerializeObject(
                        requestDto,
                        SerializerSettings
                        );
                    httpRequestMsg.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                }

                var httpRespMsg = await _client.SendAsync(httpRequestMsg);
                statusCode = (int)httpRespMsg.StatusCode;
                byte[] bytes = await httpRespMsg.Content.ReadAsByteArrayAsync();
                responseJson = Encoding.UTF8.GetString(bytes);
                //_logger.LogInformation("Send request {httpMethod} {url}.", httpMethod, uri);
                //Payload is only in debug logs.
                

                if (httpRespMsg.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Status code of response is {httpRespMsg.StatusCode}");
                }

                var jToken = JsonConvert.DeserializeObject<JToken>(responseJson);

                _logger.LogDebug(
                    message:"----> {httpMethod} {url}\n{requestJson}\n{statusCode}\n{responseJson}",
                    httpMethod,
                    uri,
                    requestJson,
                    statusCode,
                    responseJson
                );

                return jToken;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    "--/-> {httpMethod} {url}\n{requestJson}\n{statusCode}\n{responseJson}\n{exception}",
                    httpMethod,
                    uri,
                    requestJson,
                    statusCode,
                    responseJson,
                    ex
                );
                throw;
            }


        }

    }
}
