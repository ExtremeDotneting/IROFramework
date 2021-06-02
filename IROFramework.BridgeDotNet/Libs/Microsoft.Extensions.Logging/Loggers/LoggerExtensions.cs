// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// ILogger extension methods for common scenarios.
    /// </summary>
    public static class LoggerExtensions
    {
        public static void LogDebug(this ILogger logger, params object[] messageArgs)
        {
            logger.Log(LogLevel.Debug, messageArgs);
        }

        public static void LogTrace(this ILogger logger, params object[] messageArgs)
        {
            logger.Log(LogLevel.Trace, messageArgs);
        }

        public static void LogError(this ILogger logger, params object[] messageArgs)
        {
            logger.Log(LogLevel.Error, messageArgs);
        }

        public static void LogInformation(this ILogger logger, params object[] messageArgs)
        {
            logger.Log(LogLevel.Information, messageArgs);
        }

        public static void LogWarning(this ILogger logger, params object[] messageArgs)
        {
            logger.Log(LogLevel.Warning, messageArgs);
        }

        public static void LogCritical(this ILogger logger, params object[] messageArgs)
        {
            logger.Log(LogLevel.Critical, messageArgs);
        }
        

    }
}
