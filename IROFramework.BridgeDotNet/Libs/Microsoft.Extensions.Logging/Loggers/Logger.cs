using System;
using System.Collections.Generic;
using System.Linq;
using Bridge;
using Libs;
using Libs.Utils;
using Newtonsoft.Json;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Minimalistic logger that does nothing.
    /// </summary>
    public class Logger : ILogger
    {
        static long _logRecordNumber = 0;
        public string CategoryName { get; set; } = "Global";

        public void Log(LogLevel logLevel, params object[] messageArgs)
        {
            if (logLevel == LogLevel.None)
                return;

            if (messageArgs?.Any() != true)
                throw new Exception(nameof(messageArgs) + " is empty.");

            Action<object, string> writeFunc = BrowserConsole.Log;
            if (logLevel >= LogLevel.Error)
            {
                writeFunc = BrowserConsole.Error;
            }
            else if (logLevel >= LogLevel.Warning)
            {
                writeFunc = BrowserConsole.Warn;
            }
            else if (logLevel >= LogLevel.Information)
            {
                writeFunc = BrowserConsole.Info;
            }
            string metadataMsg = $"[{GetLogLevelString(logLevel)} #{_logRecordNumber}]";
            _logRecordNumber++;
            writeFunc(metadataMsg, CategoryName);

            var startIndex = 0;
            if (messageArgs[0] is string firstMsg)
            {
                startIndex = 1;
                writeFunc(firstMsg, CategoryName);
            }


            for (var i = startIndex; i < messageArgs.Length; i++)
            {
                var obj = messageArgs[i];


                if (obj is Exception exceptionObj)
                {
                    var exceptionDesc = exceptionObj.ToDetailedString();
                    writeFunc(exceptionDesc, CategoryName);
                }
                else
                {
                    var typeStr = obj.GetType().Name;
                    var logRecordContainer = new object().ToPlainObjectClone();
                    logRecordContainer.CSharpObjectType = typeStr;

                    try
                    {
                        logRecordContainer.PlainObj = obj.ToPlainObjectClone();
                    }
                    catch
                    {
                    }

                    logRecordContainer.Obj = obj;
                    writeFunc(logRecordContainer, CategoryName);
                }
            }
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return new FakeScope();
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "trce";
                case LogLevel.Debug:
                    return "dbug";
                case LogLevel.Information:
                    return "info";
                case LogLevel.Warning:
                    return "warn";
                case LogLevel.Error:
                    return "fail";
                case LogLevel.Critical:
                    return "crit";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }
    }

    /// <summary>
    /// Minimalistic logger that does nothing.
    /// </summary>
    public class Logger<T> : Logger, ILogger<T>
    {
    }
}
