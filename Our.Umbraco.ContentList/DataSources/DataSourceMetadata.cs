using System.Collections.Generic;
using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.DataSources
{
    public class DataSourceMetadata
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("parameters")]
        public List<DataSourceParameterDefinition> Parameters { get; set; }
    }
}