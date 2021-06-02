using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IROFramework.Core.Tools.LoggingExt
{
    public static class StaticLogger
    {
        static ILogger _log;
        static ILogger Log
        {
            get
            {
                if (_log == null)
                {
                    throw new Exception("StaticLogger not init.");
                }
                return _log;
            }
        }

        public static void LogError(Exception ex, string msg = "", params object[] args)
        {
            Log.LogError(ex, msg, args);
        }

        public static void LogDebug(string msg , params object[] args)
        {
            Log.LogDebug(msg, args);
        }

        public static void LogInformation(string msg, params object[] args)
        {
            Log.LogInformation(msg, args);
        }

        public static void LogWarning(string msg, params object[] args)
        {
            Log.LogWarning(msg, args);
        }

        public static void LogWarning(Exception ex, string msg = "", params object[] args)
        {
            Log.LogWarning(ex, msg, args);
        }

        public static void LogTrace(string msg, params object[] args)
        {
            Log.LogTrace(msg, args);
        }

        public static void Init()
        {
            var services = new ServiceCollection();
            services.AddLogging(conf =>
            {
                var currentLogLevel = LogLevel.Warning;
                conf.AddFilter((loggerName, categoryName, logLevel) =>
                {
                    if (categoryName.StartsWith("Microsoft"))
                        return false;
                    return true;
                });
                conf.AddConsole();
                conf.AddDebug();
                conf.SetMinimumLevel(currentLogLevel);
            });
            var sp = services.BuildServiceProvider();
            var factory = sp.GetRequiredService<ILoggerFactory>();
            var category = AppDomain.CurrentDomain.FriendlyName;
            _log = factory.CreateLogger(category);
        }

        /// <summary>
        /// Init in UseMvc or use default with debug output.
        /// </summary>
        public static void Init(ILoggerFactory loggerFactory)
        {
            if (_log != null)
                throw new Exception("Common logger was init before.");
            _log = loggerFactory.CreateLogger(typeof(StaticLogger));
        }
    }
}
