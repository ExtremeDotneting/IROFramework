using System;

namespace UndergroundIRO.ApiBase.Exceptions
{
    public class ApiException : Exception
    {
        /// <summary>
        /// Gets or sets the error code (HTTP status code)
        /// </summary>
        /// <value>The error code (HTTP status code).</value>
        public int? HttpCode { get; }

        public string ResponseContent { get; }

        public ApiException() { }

        public ApiException(string message) : base(message) { }

        public ApiException(int httpCode, string message, string responseContent = null) 
            : base(CreateMessage(httpCode, message, responseContent))
        {
            this.HttpCode = httpCode;
            this.ResponseContent = responseContent;
        }

        static string CreateMessage(int httpCode, string message, string responseContent = null)
        {
            message += $"\n------------\nWith http code: {httpCode}.";
            if (responseContent != null)
            {
                message += "\n------------\nWith response content:\n";
                message += responseContent;
            }
            return message;
        }
    }




}