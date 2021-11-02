using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Our.Umbraco.ContentList.Models
{
    public class PreviewContentListConfiguration : ContentListConfiguration
    {
        public int ContentId { get; set; }
    }
}