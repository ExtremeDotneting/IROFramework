using System;
using System.Collections.Generic;

namespace IROFramework.Web.Tools.Cron
{
    public class CronConfigs
    {
        /// <summary>
        /// CronTask - implemention of <see cref="ICronTask"/>.
        /// </summary>
        public IList<(Type CronTask, TimeSpan Delay)> CronTasks { get; set; } = new List<(Type CronTask, TimeSpan Delay)>();

        public bool IsDebug { get; set; }
    }
}