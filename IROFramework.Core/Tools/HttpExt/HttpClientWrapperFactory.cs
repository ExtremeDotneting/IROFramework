using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace IROFramework.Core.Tools.HttpExt
{
    public class HttpClientWrapperFactory
    {
        readonly ILoggerFactory _loggerFactory;

        public HttpClientWrapperFactory(ILoggerFactory logger)
        {
            _loggerFactory = logger;
        }

        public HttpClientWrapper Resolve(HttpClient client, ILogger logger)
        {
            return new HttpClientWrapper(client, logger);
        }

        public HttpClientWrapper Resolve(HttpClient client)
        {
            var logger = _loggerFactory.CreateLogger<HttpClientWrapper>();
            return new HttpClientWrapper(client, logger);
        }
    }
}