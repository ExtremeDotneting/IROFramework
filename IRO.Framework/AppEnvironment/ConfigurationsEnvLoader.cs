using IROFramework.Core.AppEnvironment.SettingsDto;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace IROFramework.Core.AppEnvironment
{
    public class ConfigurationsEnvLoader : IEnvLoader
    {
        readonly IConfiguration _config;

        public ConfigurationsEnvLoader(IConfiguration config)
        {
            _config = config;
        }

        public T GetValue<T>(string propName)
        {
            //This used to allow injecting all app settings in ine environment variable with json.
            //Used to deploy on heroku.

            var settingsInEnvProperty = _config.GetValue<string>("APP_SETTINGS");
            if (string.IsNullOrWhiteSpace(settingsInEnvProperty))
            {
                return _config.GetSection(propName).Get<T>();
            }
            else
            {
                var jToken = JToken.Parse(settingsInEnvProperty);
                return jToken[propName].ToObject<T>();
            }
        }
    }
}