using Bridge;
using Bridge.Html5;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Libs
{
    public static class BridgeCommonExtensions
    {
        /// <summary>
        /// Return plain js object without c# metadata.
        /// </summary>
        public static dynamic ToPlainObject(this object @this)
        {
            return Script.ToPlainObject(@this);
        }

        /// <summary>
        /// Return plain js object clone without c# metadata.
        /// </summary>
        public static dynamic ToPlainObjectClone(this object @this)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                //ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var json = JsonConvert.SerializeObject(@this, jsonSettings);
            return JSON.Parse(json);
        }
    }
}
