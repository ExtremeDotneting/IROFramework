using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IROFramework.Core.Consts
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DatabaseType
    {
        None,
        KeyValue, 
        LiteDB
    }
}