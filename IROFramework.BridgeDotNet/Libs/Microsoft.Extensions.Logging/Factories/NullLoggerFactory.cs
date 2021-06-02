// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.Logging
{
    public class NullLoggerFactory : ILoggerFactory
    {
        public ILogger CreateLogger(string name)
        {
            return new NullLogger();
        }

    }
}