using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Web
{
    public class ContentListColumns
    {
        [JsonProperty("small")]
        public int Small { get; set; }
        [JsonProperty("medium")]
        public int Medium { get; set; }
        [JsonProperty("large")]
        public int Large { get; set; }
    }
}