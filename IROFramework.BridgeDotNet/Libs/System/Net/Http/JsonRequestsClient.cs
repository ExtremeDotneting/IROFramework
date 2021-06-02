using Bridge.Html5;
using Bzx.Tools.ApiFacadesDevKit.Errors;
using Bzx.Tools.ApiFacadesDevKit.RequestHandling;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Bzx.Tools.ApiFacadesDevKit.Http
{
    public class JsonRequestsClient : BaseHttpRequestsClient, IRequestsClient
    {
        public int RequestsDelayMs { get; set; } = 500;

        /// <summary>
        /// Домен в стиле https://example.com
        /// </summary>
        public string DomainPrefix { get; }

        /// <summary>
        /// Составляет ссылку для запроса из префикса домена и строки роутинга.
        /// </summary>
        public IUrlRoutingResolver UrlRoutingResolver { get; }

        protected ResultHttpMessageProcessor _resultProc = new ResultHttpMessageProcessor();

        public JsonRequestsClient(string domainPrefix, IUrlRoutingResolver urlRoutingResolver = null)
        {
            DomainPrefix = domainPrefix ?? 
                throw new ArgumentNullException(nameof(domainPrefix), "Domain prefix can`t be null.");
            UrlRoutingResolver = urlRoutingResolver ?? new DefUrlRoutingResolver();
        }

        public async Task Send<TRes>(ApiResponse<TRes> preProcessedApiResponse)
        {
            //throw new AggregateException(
            //    "Top ex",
            //    new Exception("Inner ex1"),
            //    new Exception(
            //        "Inner ex2",
            //        new Exception(
            //            "Inner inner ex",
            //            new AggregateException(
            //                "##Top ex",
            //                new Exception("##Inner ex1"),
            //                new Exception(
            //                    "##Inner ex2",
            //                    new Exception("##Inner inner ex")
            //                )   
            //            )
            //        )
            //    )
            //);

            //init
            var apiRequest = preProcessedApiResponse.Request;
            string url = UrlRoutingResolver.GetUrlStart(DomainPrefix, apiRequest.Routing);
            var headers = apiRequest.Headers;
            var parameters = apiRequest.Params;
            //endinit           
            
            XMLHttpRequest httpRequest = null;
            int tooManyReqExceptionCount = 0;
            try
            {               
                //Подготовка распознавателя метаданных.  
                //Сложный асинхронный код.
                var jsonStr = JsonConvert.SerializeObject(parameters);
                Action<Event> onLoad = (ev) =>
                {
                };
                Action<Event> onError = (ev) =>
                {
                };
                preProcessedApiResponse.ResolveRequestProcessorMetadata = (dictToFill) =>
                {
                    dictToFill["TooManyReqExceptionsCount"] = tooManyReqExceptionCount;
                    dictToFill["Url"] = url;
                    ResolveMetadata(dictToFill, httpRequest);                    
                };                

                //Отправка и обработка запроса.
                while (tooManyReqExceptionCount < 4)
                {
                    //Отравляем запрос, в случае TooManyRequestsException ждем таймаут и опять отправляем.
                    //В случае успешной отправки цикл разрывается, как и в случае других исключений.
                    try
                    {

                        //Подготовка и отправка запроса.
                        httpRequest = new XMLHttpRequest();
                        httpRequest.Open("POST", url, true);
                        httpRequest.SetRequestHeader("Content-Type", "application/json");
                        AddHeaders(httpRequest, headers);
                        
                        Task task = MakeRequestAwait(
                            httpRequest,
                            onLoad,
                            onError
                            );
                        httpRequest.Send(jsonStr);
                        await task;

                        //Обработка результата.
                        var result = await ProcessResult<TRes>(httpRequest);

                        //Запись результата.
                        preProcessedApiResponse.Result = result;
                        preProcessedApiResponse.IsOk = true;
                        break;
                    }
                    catch (TooManyRequestsException)
                    {
                        tooManyReqExceptionCount++;
                        await Task.Delay(RequestsDelayMs);
                    }
                }
            }
            catch (TooManyRequestsException ex)
            {
                //preProcessedApiResponse.Exception = ex;
                //preProcessedApiResponse.IsOk = false;

                //На самом деле нет смысла перехватывать исключени и записывать их в ApiResponse,
                //т.к. его владелец, а именно - класс Api.cs сам поймает ошибку и запишет ее в ApiResponse,
                //потому достаточно просто:
                throw;

            }
        }

        async Task<TRes> ProcessResult<TRes>(XMLHttpRequest httpRequest)
        {
            return await _resultProc.ProcessResult<TRes>(httpRequest);
        }

        async Task MakeRequestAwait(XMLHttpRequest req, Action<Event> onLoadProcessor, Action<Event> onErrorProcessor)
        {
            //Просто приводим асинхронность к нормальному виду.
            var taskCompletionSource = new TaskCompletionSource<object>();
            req.OnLoad = (ev) =>
            {
                onLoadProcessor?.Invoke(ev);
                taskCompletionSource.SetResult(null);
            };
            req.OnError = (ev) =>
            {
                onErrorProcessor?.Invoke(ev);
                taskCompletionSource.SetException(new Exception("Error with XMLHttpRequest."));
            };
            await taskCompletionSource.Task;
        }
    }
}
