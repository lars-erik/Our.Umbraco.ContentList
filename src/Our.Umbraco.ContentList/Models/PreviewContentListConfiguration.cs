using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Models
{
    [ModelBinder(typeof(JsonBinder))]
    public class PreviewContentListConfiguration : ContentListConfiguration
    {
        [JsonProperty("contentId")]
        public int ContentId { get; set; }
    }
}