using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using IROFramework.Core.AppEnvironment.SettingsDto;

namespace IROFramework.Core.AppEnvironment
{
    public static class Env
    {
        static IDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();
        static IEnvLoader _envLoader;

        public static string EnvironmentName { get; private set; }

        public static GlobalSettings GlobalSettings { get; private set; }

        public static bool IsDebug => GlobalSettings.DebugEnabled;

        public static string ExternalUrl => GlobalSettings.ExternalUrl;

        public static void Init(IEnvLoader envLoader)
        {
            if (_envLoader != null)
            {
                throw new Exception("App environment class was init before.");
            }
            _envLoader = envLoader;

            EnvironmentName = GetValue<string>(nameof(EnvironmentName));
            GlobalSettings = GetValue<GlobalSettings>(nameof(GlobalSettings))
                             ?? throw new NullReferenceException($"Env.{nameof(GlobalSettings)} is null.");
        }

        public static T GetValue<T>(string propName = null)
        {
            propName ??= typeof(T).Name;
            if (_cache.TryGetValue(propName, out var cacheVal))
            {
                return (T)cacheVal;
            }
            if (_envLoader == null)
            {
                throw new Exception("Env loader not initialized.");
            }
            var value= _envLoader.GetValue<T>(propName);
            _cache[propName] = value;
            return value;
        }
    }
}
