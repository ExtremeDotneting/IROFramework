using IROFramework.Core.Consts;
using Serilog.Events;

namespace IROFramework.Core.AppEnvironment.SettingsDto
{
    public class GlobalSettings
    {
        public string ExternalUrl { get; set; }

        public bool DebugEnabled { get; set; }

        public string HashSalt { get; set; }

        /// <summary>
        /// Key value storage type.
        /// <para></para>
        /// Now available 'Ram' or 'Telegram'.
        /// </summary>
        public StorageType StorageType { get; set; }

        public DatabaseType DatabaseType { get; set; }

        public LogEventLevel LogLevel { get; set; } = LogEventLevel.Error;

        public LogEventLevel ConsoleLogLevel { get; set; } = LogEventLevel.Error;
    }
}