using System.Collections.Generic;
using Lucene.Net.Support;
using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Web
{
    public class ContentListDataSource
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("parameters")]
        public List<DataSourceParameterValue> Parameters { get; set; } = new List<DataSourceParameterValue>();

    }
}