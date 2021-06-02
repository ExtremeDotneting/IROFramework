using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IROFramework.Core.Consts
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StorageType
    {
        None,
        Ram, 
        Telegram
    }
}