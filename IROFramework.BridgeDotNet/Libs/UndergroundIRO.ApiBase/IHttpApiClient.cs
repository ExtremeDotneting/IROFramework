using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UndergroundIRO.ApiBase.Models;

namespace UndergroundIRO.ApiBase
{
    public interface IHttpApiClient
    {
        string Id { get; }
        int TotalRequestsCount { get; }
        HttpConfiguration Configuration { get; }
        JsonSerializerSettings JsonSettings { get; set; }

        Task<HttpResponseMessage> CallApiAsync(
            string absoluteUrl,
            HttpMethod method,
            IDictionary<string, string> headerParams = null
        );

        Task<HttpResponseMessage> CallApiAsync(
            string absoluteUrl,
            HttpMethod method,
            string stringsBody,
            string mediaType = "application/json",
            Encoding textContentEncoding = null,
            IDictionary<string, string> headerParams = null
        );

        Task<HttpResponseMessage> CallApiAsync(
            string absoluteUrl,
            HttpMethod method,
            IDictionary<string, string> formParams,
            IDictionary<string, string> headerParams 
        );

        Task<HttpResponseMessage> CallApiAsync(
            string absoluteUrl,
            HttpMethod method,
            IDictionary<string, string> headerParams,
            HttpContent httpContent
        );

        /// <summary>
        /// Will throw exception if can't.
        /// </summary>
        Task<ApiResponse<T>> ResolveApiResponse<T>(HttpResponseMessage httpResponse, JsonSerializerSettings settings = null);

        void AddDefaultHeader(string key, string value);
        string RelativeUrlToAbsolute(string basePath, string relativeUrl);

        T Deserialize<T>(string str, JsonSerializerSettings settings = null);
        string SerializeAsType<T>(T obj, JsonSerializerSettings settings = null);
        string Serialize(object obj, JsonSerializerSettings settings = null);
        IDictionary<string, string> ParametersToDict(object obj, JsonSerializerSettings settings = null);
        string ParameterToString(object obj, JsonSerializerSettings settings = null);
        string RemoveJsonStringBrackets(string json);
        Task<ApiResponse<TResult>> GetRequest<TResult>(string absoluteUrl);

        /// <summary>
        /// </summary>
        /// <param name="requestData">Serialized as json.</param>
        Task<ApiResponse<TResult>> PostRequest<TRequest, TResult>(string absoluteUrl, TRequest requestData);

        /// <summary>
        /// </summary>
        /// <param name="requestData">Serialized as json.</param>
        Task<ApiResponse<TResult>> PutRequest<TRequest, TResult>(string absoluteUrl, TRequest requestData);

        /// <summary>
        /// </summary>
        /// <param name="requestData">Serialized as json.</param>
        Task<ApiResponse<TResult>> HeadRequest<TRequest, TResult>(string absoluteUrl, TRequest requestData);

        /// <summary>
        /// </summary>
        /// <param name="requestData">Serialized as json.</param>
        Task<ApiResponse<TResult>> DeleteRequest<TRequest, TResult>(string absoluteUrl, TRequest requestData);

        /// <summary>
        /// </summary>
        /// <param name="requestData">Serialized as json.</param>
        Task<ApiResponse<TResult>> OptionsRequest<TRequest, TResult>(string absoluteUrl, TRequest requestData);
    }
}