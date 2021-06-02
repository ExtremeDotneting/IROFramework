using System;
using System.Linq;
using System.Threading.Tasks;
using IRO.Storage;
using IROFramework.Core.Tools.LoggingExt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace IROFramework.Web.Tools.Cron
{
    public static class LazyCronMiddleware
    {
        /// <summary>
        /// Execute cron task on http-request if delay passed.
        /// <para></para>
        /// All cron tasks must be registered in service collection and must implement <see cref="ICronTask"/>.
        /// </summary>
        public static void UseLazyCron(this IApplicationBuilder app, Action<CronConfigs> confAct)
        {
            var conf = new CronConfigs();
            confAct.Invoke(conf);
            var cronTasks = conf.CronTasks;
            if (!cronTasks.Any())
                return;
            var globalSP = app.ApplicationServices;
            var storage = globalSP.GetRequiredService<IKeyValueStorage>();
            //Offcet used to prevent parallel cron tasks launch when scaling on multiple servers.
            var randomOffset = TimeSpan.FromSeconds(new Random().Next(20));

            app.Use(async (ctx, next) =>
            {
                try
                {
                    foreach (var cronTaskInfo in cronTasks)
                    {
                        var key = $"CRON_TASK_{cronTaskInfo.CronTask.Name}_LAST_LAUNCH";
                        var lastLaunch = (await storage.GetOrDefault<DateTime?>(key)) ?? DateTime.MinValue;
                        if (DateTime.UtcNow + randomOffset > lastLaunch + cronTaskInfo.Delay)
                        {
                            await storage.Set(key, DateTime.UtcNow);
                            //Execute cron task in new thread.
                            Func<Task> func = async () =>
                            {
                                try
                                {
                                    var cronTask = (ICronTask) globalSP.GetRequiredService(cronTaskInfo.CronTask);
                                    await cronTask.Handle();
                                }
                                catch (Exception ex)
                                {
                                    StaticLogger.LogError(ex);
                                    if (conf.IsDebug)
                                        throw;
                                }
                            };
                            if (conf.IsDebug)
                            {
                                await func.Invoke();
                            }
                            else
                            {
                                var task = Task.Run(func);
                            }
                        }
                    }
                }
                finally
                {
                    await next();
                }
            });

        }

    }
}