using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Models
{
    public class DataSourceParameterValue : IParameterValue
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("value")]
        public object Value { get; set; }
    }
}