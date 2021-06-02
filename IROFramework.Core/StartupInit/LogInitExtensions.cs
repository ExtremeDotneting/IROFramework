using System;
using IROFramework.Core.AppEnvironment;
using IROFramework.Core.Tools.LoggingExt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace IROFramework.Core.StartupInit
{
    public static class LogInitExtensions
    {
        public static void AddMyLogging(this IServiceCollection services)
        {
            services.AddLogging(logBuilder =>
            {
                var currentLogLevel = (LogLevel)Enum.Parse(
                    typeof(LogLevel),
                    Env.GlobalSettings.LogLevel.ToString()
                    );
                logBuilder.AddFilter((loggerName, categoryName, logLevel) =>
                {
                    if (categoryName.StartsWith("TabletopHelperSite"))
                        return true;
                    if (categoryName.StartsWith(nameof(IROFramework)))
                        return true;
                    if (categoryName.StartsWith(nameof(IRO)))
                        return true;
                    if (categoryName.StartsWith("UndergroundIRO"))
                        return true;
                    return false;
                });
                logBuilder.SetMinimumLevel(currentLogLevel);
                logBuilder.ClearProviders();

                //logBuilder.AddConsole((opt) =>
                //{
                //    opt.LogToStandardErrorThreshold = LogLevel.Error;
                //    opt.IncludeScopes = false;
                //});
                //logBuilder.AddDebug();

                var serilog = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.Debug(Env.GlobalSettings.LogLevel)
                    .WriteTo.Console(Env.GlobalSettings.ConsoleLogLevel)
                    .CreateLogger();
                logBuilder.AddSerilog(serilog);
            });
        }

        public static void InitMyLogging(this IApplicationBuilder app)
        {
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            StaticLogger.Init(loggerFactory);
        }
    }
}
