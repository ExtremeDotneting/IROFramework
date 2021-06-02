using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge;
using Bridge.Html5;
using Libs;
using Libs.Libs.System.Net.Http.Exceptions;
using Microsoft.Extensions.Logging;

namespace System.Net.Http
{
    public class HttpClient
    {
        readonly ILogger _log;

        public HttpClient(ILoggerFactory loggerFactory = null)
        {
            loggerFactory = loggerFactory ?? new LoggerFactory();
            _log = loggerFactory.CreateLogger(GetType());
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
        {
            if (requestMessage == null)
                throw new ArgumentNullException(nameof(requestMessage));
            if (requestMessage.RequestUri == null)
                throw new ArgumentNullException(nameof(requestMessage) + nameof(requestMessage.RequestUri));

            var httpReq = new XMLHttpRequest();
            var url = requestMessage.RequestUri.ToString();

            var method = requestMessage
                .Method
                .AsString()
                .ToUpper();
            httpReq.Open(method, url, true);

            if (requestMessage.Headers != null)
            {
                AddHeaders(httpReq, requestMessage.Headers);
            }
            var task = MakeRequestAwaitable(httpReq);
            var content = requestMessage.Content;
            if (content == null)
            {
                httpReq.Send();
            }
            else
            {
                httpReq.SetRequestHeader("Content-Type", requestMessage.Content.ContentType);
                if (content is StringContent stringsHttpContent)
                {
                    httpReq.Send(stringsHttpContent.Content);
                }
                else if (content is FormUrlEncodedContent formContent)
                {
                    httpReq.Send(formContent.Content);
                }
                else
                {
                    httpReq
                        .ToDynamic()
                        .Send(content.GetContent());
                }
            }

            try
            {
                await task;
            }
            catch
            {
                //Error will be handled below.
            }

            //Processing response.
            //_log.LogDebug("Resp headers: ", respHeadersStr);
            HttpContent respContent;
            if (httpReq.ResponseText != null)
            {
                respContent = new StringContent(httpReq.ResponseText);
            }
            else
            {
                switch (httpReq.ResponseType)
                {
                    case XMLHttpRequestResponseType.Json:
                    case XMLHttpRequestResponseType.Text:
                    case XMLHttpRequestResponseType.String:
                        respContent = new StringContent(httpReq.ResponseText);
                        break;
                    case XMLHttpRequestResponseType.Document:
                        respContent = new StringContent(httpReq.ResponseXML.TextContent);
                        break;

                    default:
                        respContent = new HttpContent<object>()
                        {
                            Content = httpReq.Response
                        };
                        break;
                }
            }

            var statusCode = httpReq.Status;
            var respHeadersStr = httpReq.GetAllResponseHeaders();
            var respHeaders = ParseHeaders(respHeadersStr);
            string contentType = "unknown";
            if (respHeaders.TryGetValue("content-type", out var respContentType))
            {
                contentType = respContentType;
            }
            respContent.ContentType = contentType;

            var httpResponse = new HttpResponseMessage()
            {
                InternalRequest = httpReq,
                StatusCode = statusCode,
                Content = respContent,
                Headers = respHeaders
            };
            return httpResponse;
        }

        async Task<Event> MakeRequestAwaitable(XMLHttpRequest req)
        {
            //Просто приводим асинхронность к нормальному виду.
            var taskCompletionSource = new TaskCompletionSource<Event>();
            req.OnLoad = (ev) =>
            {
                taskCompletionSource.SetResult(ev);
            };
            req.OnError = (ev) =>
            {
                _log.LogError(req);
                taskCompletionSource.SetException(new XMLHttpRequestException("Error with XMLHttpRequest.", req));

            };
            req.OnTimeout = (ev) =>
            {
                _log.LogError(req);
                taskCompletionSource.SetException(new XMLHttpRequestException("Timeout in XMLHttpRequest.", req));

            };
            req.OnAbort = (ev) =>
            {
                _log.LogError(req);
                taskCompletionSource.SetException(new XMLHttpRequestException("Aborted in XMLHttpRequest.", req));

            };
            return await taskCompletionSource.Task;
        }

        IDictionary<string, string> ParseHeaders(string headersStr)
        {
            var dict = new PlainDictionary<string, string>();

            Script.Write(@"
                var parseHeadersFunc = function (headers) {
                var trim = function (string) {
                    return string.replace(/^\s+|\s+$/g, '');
                }
                var isArray = function (arg) {
                    return Object.prototype.toString.call(arg) === '[object Array]';
                }

                if (!headers)
                    return {}

                var result = {}

                var headersArr = trim(headers).split('\n')

                for (var i = 0; i < headersArr.length; i++) {
                    var row = headersArr[i]
                    var index = row.indexOf(':'),
                        key = trim(row.slice(0, index)).toLowerCase(),
                        value = trim(row.slice(index + 1))

                    if (typeof (result[key]) === 'undefined') {
                        result[key] = value
                    } else if (isArray(result[key])) {
                        result[key].push(value)
                    } else {
                        result[key] = [result[key], value]
                    }
                }

                return result;
            }");
            var parsedHeadersObj = Script.Call<object>("parseHeadersFunc", headersStr);
            var keys = Script.GetOwnPropertyNames(parsedHeadersObj);
            foreach (var key in keys)
            {
                var value = parsedHeadersObj[key];
                dict[key] = value.ToString();
            }
            return dict;
        }

        void AddHeaders(XMLHttpRequest httpRequest, IDictionary<string, string> headers)
        {
            if (headers == null)
            {
                return;
            }

            foreach (var item in headers)
            {
                if (item.Value == null)
                    continue;
                httpRequest.SetRequestHeader(item.Key, item.Value);
            }
        }
    }


}
