namespace Microsoft.Extensions.Logging
{
    public class LoggerFactory : ILoggerFactory
    {

        public ILogger CreateLogger(string name)
        {
            var log = new Logger();
            log.CategoryName = name;
            return log;
        }

    }
}