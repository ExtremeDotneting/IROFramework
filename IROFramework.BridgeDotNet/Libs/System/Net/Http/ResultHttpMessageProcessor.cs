using Bridge;
using Bridge.Html5;
using Bzx.Tools.ApiFacadesDevKit.Errors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bzx.Tools.ApiFacadesDevKit.RequestHandling
{
    /// <summary>
    /// Внимание! Здесь будут реализованы только самые глобальные обработчики ошибок, вроде обработки http кодов.
    /// Используйте наследование для добавления функционала.
    /// </summary>
    public class ResultHttpMessageProcessor
    {
        List<Action<XMLHttpRequest, object>> _willThrowIfErrorActions;

        public ResultHttpMessageProcessor()
        {
            _willThrowIfErrorActions = new List<Action<XMLHttpRequest, object>>()
            {
                ThrowIfErrorHttpCode,
                ThrowIfErrorInJson
            };
        }        

        /// <summary>
        /// Либо вернет результат, либо выкинет ошибку.
        /// </summary>
        public async Task<TRes> ProcessResult<TRes>(XMLHttpRequest httpRequest)
        {
            var exceptions = new List<Exception>();
            object response = null;
            TRes result=default(TRes);
            try
            {
                var content = httpRequest.ResponseText;
                JsonSerializerSettings jss = new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                };
                response = JsonConvert.DeserializeObject<object>(content, jss);
                result = JsonConvert.DeserializeObject<TRes>(content, jss);

            }
            catch(Exception ex)
            {
                exceptions.Add(ex);
            }          
            
            foreach (var act in _willThrowIfErrorActions)
            {
                try
                {
                    act(httpRequest, response);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count == 1)
                throw exceptions[0];
            if (exceptions.Count > 1)
            {
                throw new AggregateException(exceptions);
            }
            return result;
        }

        void ThrowIfErrorHttpCode(XMLHttpRequest httpRequest,  object response)
        {
            var code = (int)httpRequest.Status;
            if (code==0 || (code >= 400 && code < 600))
            {
                throw new HttpCodeException(code);
            }
        }

        void ThrowIfErrorInJson(XMLHttpRequest httpRequest, object response)
        {
            bool isError = false;
            try
            {
                isError = response["isError"].As<bool>();
            }
            catch{ }

            if (isError)
            {
                throw new Exception("Error in api : " + response["errorMsg"]);
            }
        }
    }
}
