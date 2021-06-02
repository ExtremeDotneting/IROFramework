using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace IROFramework.Core.AppEnvironment
{
    public class FileEnvLoader : IEnvLoader
    {
        readonly JToken _appSettingsJToken;

        public FileEnvLoader(string jsonFilePath=null)
        {
            jsonFilePath ??= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            var json = File.ReadAllText(jsonFilePath);
            _appSettingsJToken = JToken.Parse(json);
        }

        public T GetValue<T>(string propName)
        {
            return _appSettingsJToken[propName].ToObject<T>();
        }
    }
}