using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bridge.Html5;
using Bridge;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UndergroundIRO.ApiBase.Models;
using UndergroundIRO.ApiBase.Services;

namespace UndergroundIRO.ApiBase
{
    public partial class HttpApiClient : IHttpApiClient
    {
        public string Id { get; }
        public int TotalRequestsCount { get; private set; }
        public HttpConfiguration Configuration { get; }
        public JsonSerializerSettings JsonSettings { get; set; } = new JsonSerializerSettings();

        protected ILogger Log { get; }
        protected HttpClient HttpClient { get; }

        protected IExceptionsFactory ExceptionsFactory;

        public HttpApiClient(
            HttpClient httpClient = null,
            HttpConfiguration conf = null,
            IExceptionsFactory exceptionsFactory = null,
            ILoggerFactory loggerFactory = null
            )
        {
            HttpClient = httpClient ?? new HttpClient();
            Id = Guid.NewGuid().ToString().Remove(6);
            Configuration = conf ?? new HttpConfiguration();
            loggerFactory = loggerFactory ?? new LoggerFactory();
            Log = loggerFactory.CreateLogger(GetType());
            ExceptionsFactory = exceptionsFactory ?? new ExceptionsFactory();
        }

        #region CallApi.
        public async Task<HttpResponseMessage> CallApiAsync(
            string absoluteUrl,
            HttpMethod method,
            IDictionary<string, string> headerParams = null
        )
        {
            return await CallApiAsync_Privat(
                absoluteUrl: absoluteUrl,
                method: method,
                headerParams: headerParams,
                formParams: null,
                textContentEncoding: null,
                mediaType: null,
                stringsBody: null,
                httpContent: null
            );
        }

        public async Task<HttpResponseMessage> CallApiAsync(
            string absoluteUrl,
            HttpMethod method,
            string stringsBody,
            string mediaType = "application/json",
            Encoding textContentEncoding = null,
            IDictionary<string, string> headerParams = null
        )
        {
            return await CallApiAsync_Privat(
                absoluteUrl: absoluteUrl,
                method: method,
                headerParams: headerParams,
                formParams: null,
                textContentEncoding: textContentEncoding,
                mediaType: mediaType,
                stringsBody: stringsBody,
                httpContent: null
            );
        }

        public async Task<HttpResponseMessage> CallApiAsync(
            string absoluteUrl,
            HttpMethod method,
            IDictionary<string, string> formParams,
            IDictionary<string, string> headerParams
        )
        {
            return await CallApiAsync_Privat(
                absoluteUrl: absoluteUrl,
                method: method,
                headerParams: headerParams,
                formParams: formParams,
                textContentEncoding: null,
                mediaType: null,
                stringsBody: null,
                httpContent: null
            );
        }

        public async Task<HttpResponseMessage> CallApiAsync(
            string absoluteUrl,
            HttpMethod method,
            IDictionary<string, string> headerParams,
            HttpContent httpContent
        )
        {
            return await CallApiAsync_Privat(
                absoluteUrl: absoluteUrl,
                method: method,
                headerParams: headerParams,
                formParams: null,
                textContentEncoding: null,
                mediaType: null,
                stringsBody: null,
                httpContent: httpContent
            );
        }

        async Task<HttpResponseMessage> CallApiAsync_Privat(
            string absoluteUrl,
            HttpMethod method,
            IDictionary<string, string> headerParams,
            IDictionary<string, string> formParams,
            Encoding textContentEncoding,
            string mediaType,
            string stringsBody,
            HttpContent httpContent
        )
        {
            var request = PrepareRequest(
                url: absoluteUrl,
                method: method,
                headerParams: headerParams,
                formParams: formParams,
                textContentEncoding: textContentEncoding,
                mediaType: mediaType,
                stringsBody: stringsBody,
                httpContent: httpContent
            );
            InterceptRequest(request);
            TotalRequestsCount++;

            var logStr = $"Request number '{TotalRequestsCount}' parameters is:\n";
            Log.LogDebug(logStr, request);

            var response = await HttpClient.SendAsync(request);
            Log.LogDebug(response);
            await ThrowIfResponseError(request, response);
            InterceptResponse(request, response);
            return response;
        }
        #endregion

        /// <summary>
        /// Will throw exception if can't.
        /// </summary>
        public virtual async Task<ApiResponse<T>> ResolveApiResponse<T>(HttpResponseMessage httpResponse, JsonSerializerSettings settings = null)
        {
            settings = settings ?? JsonSettings;
            try
            {
                var content = httpResponse.Content.ReadAsString();
                var jToken = Deserialize<object>(content, settings);
                await ThrowIfResponseError(jToken);
                var data = Deserialize<T>(content, settings);
                var apiResponse = new ApiResponse<T>(
                    httpResponse.Headers,
                    data,
                    httpResponse
                    );
                Log.LogDebug("ApiResponse:\n", apiResponse);
                return apiResponse;
            }
            catch (Exception ex)
            {
                throw new Exception("Can't deserialize api response.", ex);
            }
        }

        #region Helpers.

        public void AddDefaultHeader(string key, string value)
        {
            Configuration.DefaultHeaders.Add(key, value);
        }

        public string RelativeUrlToAbsolute(string basePath, string relativeUrl)
        {
            if (relativeUrl.StartsWith("https://") || relativeUrl.StartsWith("http://"))
            {
                return relativeUrl;
            }
            if (basePath.EndsWith("/") && relativeUrl.StartsWith("/"))
            {
                relativeUrl = relativeUrl.Substring(1);
            }
            return basePath + relativeUrl;
        }

        public virtual T Deserialize<T>(string str, JsonSerializerSettings settings = null)
        {
            if (typeof(T) == typeof(object))
            {
                return (T)JSON.Parse(str);
            }
            settings = settings ?? JsonSettings;
            return JsonConvert.DeserializeObject<T>(str, settings);
        }

        public virtual string SerializeAsType<T>(T obj, JsonSerializerSettings settings = null)
        {
            settings = settings ?? JsonSettings;
            return JsonConvert.SerializeObject(obj, typeof(T), settings);
        }

        public virtual string Serialize(object obj, JsonSerializerSettings settings = null)
        {
            settings = settings ?? JsonSettings;
            return JsonConvert.SerializeObject(obj, settings);
        }

        public IDictionary<string, string> ParametersToDict(object obj, JsonSerializerSettings settings = null)
        {
            var dict = new PlainDictionary<string, string>();
            var objDynamic = obj.ToDynamic();
            var propertyNames = Script.GetOwnPropertyNames(obj);
            foreach (var propName in propertyNames)
            {
                var propValue = objDynamic[propName];
                dict[propName] = ParameterToString(propValue, settings);
            }
            return dict;
        }

        public string ParameterToString(object obj, JsonSerializerSettings settings = null)
        {
            settings = settings ?? this.JsonSettings;
            if (obj == null)
            {
                return "null";
            }
            else if (obj is string)
            {
                return (string)obj;
            }
            else if (obj is IConvertible)
            {
                var res = Serialize(obj, settings);
                return RemoveJsonStringBrackets(res);
            }
            else if (obj is IEnumerable enumerable)
            {
                var flattenedstring = new StringBuilder();
                foreach (var param in enumerable)
                {
                    if (flattenedstring.Length > 0)
                        flattenedstring.Append(",");
                    flattenedstring.Append(param);
                }
                return flattenedstring.ToString();
            }
            else
            {
                var res = Serialize(obj, settings);
                return RemoveJsonStringBrackets(res);
            }
        }

        public string RemoveJsonStringBrackets(string json)
        {
            if (json.StartsWith("\"") && json.EndsWith("\""))
            {
                json = json.Substring(1);
                json = json.Remove(json.Length - 1);
            }
            return json;
        }
        #endregion

        async Task ThrowIfResponseError(HttpRequestMessage request, HttpResponseMessage response)
        {
            var ex = await ExceptionsFactory.CheckHttpResponse(request, response);
            if (ex != null)
            {
                ////Logging.
                //if (CurrentLogLevel <= LogLevel.Error)
                //{
                //    Log.LogError($"{nameof(HttpApiClient)} '{Id}' RestResponse error:\n{ex}");
                //}
                throw ex;
            }
        }

        async Task ThrowIfResponseError(object responseJToken)
        {
            var ex = await ExceptionsFactory.CheckJTokenResponse(responseJToken);
            if (ex != null)
            {
                ////Logging.
                //if (CurrentLogLevel <= LogLevel.Error)
                //{
                //    Log.LogError($"{nameof(HttpApiClient)} '{Id}' JToken error:\n{ex}");
                //}
                throw ex;
            }
        }

        #region Standart.
        public virtual async Task<ApiResponse<TResult>> GetRequest<TResult>(string absoluteUrl)
        {
            var restResponse = await CallApiAsync(
                absoluteUrl,
                HttpMethod.Get
            );
            var apiResponse = await ResolveApiResponse<TResult>(restResponse);
            return apiResponse;
        }

        /// <summary>
        /// </summary>
        /// <param name="requestData">Serialized as json.</param>
        public virtual async Task<ApiResponse<TResult>> PostRequest<TRequest, TResult>(string absoluteUrl, TRequest requestData)
        {
            var body = Serialize(requestData);
            var restResponse = await CallApiAsync(
                absoluteUrl,
                HttpMethod.Post,
                stringsBody: body
            );
            var apiResponse = await ResolveApiResponse<TResult>(restResponse);
            return apiResponse;
        }

        /// <summary>
        /// </summary>
        /// <param name="requestData">Serialized as json.</param>
        public virtual async Task<ApiResponse<TResult>> PutRequest<TRequest, TResult>(string absoluteUrl, TRequest requestData)
        {
            var body = Serialize(requestData);
            var restResponse = await CallApiAsync(
                absoluteUrl,
                HttpMethod.Put,
                stringsBody: body
            );
            var apiResponse = await ResolveApiResponse<TResult>(restResponse);
            return apiResponse;
        }

        /// <summary>
        /// </summary>
        /// <param name="requestData">Serialized as json.</param>
        public virtual async Task<ApiResponse<TResult>> HeadRequest<TRequest, TResult>(string absoluteUrl, TRequest requestData)
        {
            var body = Serialize(requestData);
            var restResponse = await CallApiAsync(
                absoluteUrl,
                HttpMethod.Head,
                stringsBody: body
            );
            var apiResponse = await ResolveApiResponse<TResult>(restResponse);
            return apiResponse;
        }

        /// <summary>
        /// </summary>
        /// <param name="requestData">Serialized as json.</param>
        public virtual async Task<ApiResponse<TResult>> DeleteRequest<TRequest, TResult>(string absoluteUrl, TRequest requestData)
        {
            var body = Serialize(requestData);
            var restResponse = await CallApiAsync(
                absoluteUrl,
                HttpMethod.Delete,
                stringsBody: body
            );
            var apiResponse = await ResolveApiResponse<TResult>(restResponse);
            return apiResponse;
        }

        /// <summary>
        /// </summary>
        /// <param name="requestData">Serialized as json.</param>
        public virtual async Task<ApiResponse<TResult>> OptionsRequest<TRequest, TResult>(string absoluteUrl, TRequest requestData)
        {
            var body = Serialize(requestData);
            var restResponse = await CallApiAsync(
                absoluteUrl,
                HttpMethod.Options,
                stringsBody: body
            );
            var apiResponse = await ResolveApiResponse<TResult>(restResponse);
            return apiResponse;
        }
        #endregion
    }

}
