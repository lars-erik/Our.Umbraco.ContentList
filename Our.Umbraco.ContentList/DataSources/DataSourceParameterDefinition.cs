using System.Collections.Generic;
using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.DataSources
{
    public class DataSourceParameterDefinition
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("view")]
        public string View { get; set; }

        [JsonProperty("options")]
        public IList<DataSourceParameterOption> Options { get; set; }
    }

    public class DataSourceParameterOption
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
