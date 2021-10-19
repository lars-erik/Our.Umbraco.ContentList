using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Models
{
    public class ContentListDataSource
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("parameters")]
        public List<DataSourceParameterValue> Parameters { get; set; } = new List<DataSourceParameterValue>();

    }
}
