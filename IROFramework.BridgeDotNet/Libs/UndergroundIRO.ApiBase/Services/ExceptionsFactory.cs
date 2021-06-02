using System;
using System.Net.Http;
using System.Threading.Tasks;
using UndergroundIRO.ApiBase.Exceptions;

namespace UndergroundIRO.ApiBase.Services
{
    public class ExceptionsFactory : IExceptionsFactory
    {
        public virtual async Task<Exception> CheckHttpResponse(HttpRequestMessage request, HttpResponseMessage response)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (response == null)
                throw new ArgumentNullException(nameof(response));
            var status = (int)response.StatusCode;
            var content = response.Content.ReadAsString();
            if (status >= 400)
            {

                return new ApiException(
                    status,
                    $"Error calling '{request.RequestUri.AbsoluteUri}' method.",
                    content
                    );
            }
            if (status == 0)
            {
                return new ApiException(
                    status,
                    $"Error calling '{request.RequestUri.AbsoluteUri}' method.",
                    content
                    );
            }
            return null;
        }

        public virtual async Task<Exception> CheckJTokenResponse(object jToken)
        {
            return null;
        }
    }
}