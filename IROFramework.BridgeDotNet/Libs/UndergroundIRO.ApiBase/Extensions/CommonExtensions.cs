using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace UndergroundIRO.ApiBase.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Add if not null.
        /// </summary>
        public static void AddOptional<TValue>(this Dictionary<string, TValue> parameters, string key, TValue value)
        {
            if (value != null)
                parameters[key] = value;
        }
    }
}
