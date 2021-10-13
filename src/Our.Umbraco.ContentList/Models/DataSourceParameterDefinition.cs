using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Models
{
    public class DataSourceParameterDefinition
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("view")]
        public string View { get; set; }

        [JsonProperty("config")]
        public DataSourceConfig Config { get; set; }
    }

    public class DataSourceConfig
    {

        [JsonProperty("items")]
        public IList<DataSourceParameterOption> Items { get; set; }

    }

    public class DataSourceParameterOption
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
