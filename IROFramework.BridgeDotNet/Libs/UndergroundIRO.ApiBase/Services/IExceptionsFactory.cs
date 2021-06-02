using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace UndergroundIRO.ApiBase.Services
{
    public interface IExceptionsFactory
    {
        Task<Exception> CheckHttpResponse(HttpRequestMessage request, HttpResponseMessage response);

        Task<Exception> CheckJTokenResponse(object jToken);
    }
}
